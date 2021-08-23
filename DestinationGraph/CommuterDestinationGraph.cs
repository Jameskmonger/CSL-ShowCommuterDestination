using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    public class CommuterDestinationGraph
    {
        public static DestinationGraph GenerateGraph(ushort originalStopId)
        {
            // this method is a bit of a clone of LoadPassengers from BusAI

            ushort nextStop = TransportLine.GetNextStop(originalStopId);
            float num1 = 64f;

            var CITIZEN_AT_STOP_RANGE = (double)num1 * (double)num1;

            Vector3 stopPosition = Bridge.GetStopPosition(originalStopId);
            Vector3 nextStopPosition = Bridge.GetStopPosition(nextStop);

            int num2 = Mathf.Max((int)(((double)stopPosition.x - (double)num1) / 8.0 + 1080.0), 0);
            int num3 = Mathf.Max((int)(((double)stopPosition.z - (double)num1) / 8.0 + 1080.0), 0);
            int num4 = Mathf.Min((int)(((double)stopPosition.x + (double)num1) / 8.0 + 1080.0), 2159);
            int num5 = Mathf.Min((int)(((double)stopPosition.z + (double)num1) / 8.0 + 1080.0), 2159);

            // lookup stops by id for improved performance
            Dictionary<ushort, DestinationStop> stops = new Dictionary<ushort, DestinationStop>();

            for (int index1 = num3; index1 <= num5; ++index1)
            {
                for (int index2 = num2; index2 <= num4; ++index2)
                {
                    ushort citizenInstanceId = Singleton<CitizenManager>.instance.m_citizenGrid[index1 * 2160 + index2];
                    while (citizenInstanceId != (ushort)0)
                    {
                        CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[(int)citizenInstanceId];

                        ushort nextGridInstance = citizen.m_nextGridInstance;

                        var citizenIsAtStop = Bridge.IsCitizenInRangeOfStop(citizenInstanceId, originalStopId, CITIZEN_AT_STOP_RANGE);

                        if (
                            (citizen.m_flags & CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None
                            && citizenIsAtStop
                            && citizen.Info.m_citizenAI.TransportArriveAtSource(citizenInstanceId, ref citizen, stopPosition, nextStopPosition)
                        )
                        {
                            var destinationStopId = GetDestinationStopId(originalStopId, citizen);

                            if (!stops.ContainsKey(destinationStopId))
                            {
                                stops.Add(destinationStopId, new DestinationStop(destinationStopId, Bridge.GetStopPosition(destinationStopId)));
                            }

                            var targetBuildingId = citizen.m_targetBuilding;

                            stops[destinationStopId].AddJourney(targetBuildingId, Bridge.GetBuildingPosition(targetBuildingId));
                        }

                        citizenInstanceId = nextGridInstance;
                    }
                }
            }

            return new DestinationGraph(stops.Select(j => j.Value));
        }

        /**
         *  Get the closest destination stop for a citizen, given their origin stop
         *  
         *  most of this is ripped from TransportArriveAtTarget
         */
        private static ushort GetDestinationStopId(ushort originalStopId, CitizenInstance citizen)
        {
            ushort currentStop = TransportLine.GetNextStop(originalStopId);

            while (true)
            {
                var nextStop = TransportLine.GetNextStop(currentStop);

                // handle last stop on line - is this necessary? watch out for bugs
                if (nextStop == 0)
                {
                    return currentStop;
                }

                if (StopIsDestination(currentStop, nextStop, citizen))
                {
                    return currentStop;
                }

                currentStop = nextStop;
            }
        }

        private static bool StopIsDestination(ushort currentStop, ushort nextStop, CitizenInstance citizenData)
        {
            var currentPosition = Bridge.GetStopPosition(currentStop);
            var nextPosition = Bridge.GetStopPosition(nextStop);

            PathManager pathManager = Singleton<PathManager>.instance;
            NetManager netManager = Singleton<NetManager>.instance;

            if ((citizenData.m_flags & CitizenInstance.Flags.OnTour) == CitizenInstance.Flags.OnTour)
            {
                if ((citizenData.m_flags & CitizenInstance.Flags.TargetIsNode) == CitizenInstance.Flags.TargetIsNode)
                {
                    ushort targetStop = citizenData.m_targetBuilding;
                    if (targetStop != (ushort)0 && (double)Vector3.SqrMagnitude(netManager.m_nodes.m_buffer[(int)targetStop].m_position - currentPosition) < 4.0)
                    {
                        ushort stopAfterTarget = TransportLine.GetNextStop(targetStop);
                        if (stopAfterTarget != (ushort)0 && (double)Vector3.SqrMagnitude(netManager.m_nodes.m_buffer[(int)stopAfterTarget].m_position - nextPosition) < 4.0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (citizenData.m_path != 0U)
            {
                citizenData.m_pathPositionIndex += (byte)2;
                if ((int)citizenData.m_pathPositionIndex >> 1 >= (int)pathManager.m_pathUnits.m_buffer[(int)citizenData.m_path].m_positionCount)
                {
                    pathManager.ReleaseFirstUnit(ref citizenData.m_path);
                    citizenData.m_pathPositionIndex = (byte)0;
                }
            }

            if (citizenData.m_path != 0U)
            {
                PathUnit.Position position;
                if (pathManager.m_pathUnits.m_buffer[(int)citizenData.m_path].GetPosition((int)citizenData.m_pathPositionIndex >> 1, out position))
                {
                    citizenData.m_lastPathOffset = position.m_offset;

                    uint laneId = PathManager.GetLaneID(position);
                    if ((double)Vector3.SqrMagnitude(netManager.m_lanes.m_buffer[(int)laneId].CalculatePosition((float)citizenData.m_lastPathOffset * 0.003921569f) - nextPosition) < 4.0)
                    {
                        //if (!forceUnload)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}

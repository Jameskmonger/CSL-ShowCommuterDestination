﻿using ColossalFramework;
using UnityEngine;

namespace CSLShowCommuterDestination.Game
{
    /// <summary>
    /// The <c>Bridge</c> class is responsible for interactions with the Cities: Skylines game engine
    /// 
    /// This keeps a clear separation between the mod functionality and the hackish code that is sometimes required to work with the game
    /// </summary>
    public class Bridge
    {
        /**
         * Gets the {@link NetNode} for the given stop ID.
         */
        public static NetNode GetStopNode(ushort stopId)
        {
            return Singleton<NetManager>.instance.m_nodes.m_buffer[stopId];
        }

        /**
         * Gets the position for the given stop ID.
         */
        public static Vector3 GetStopPosition(ushort stopId)
        {
            return GetStopNode(stopId).m_position;
        }

        /**
         * Gets the count of passengers currently waiting at the given stop ID.
         */
        public static int GetStopPassengerCount(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].CalculatePassengerCount(stopId);
        }

        /**
         * Gets the index of the stop within the line
         */
        public static int GetStopIndex(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            ushort currentStop = Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].m_stops;

            int index = 1;
            while (currentStop != 0)
            {
                if (currentStop == stopId)
                {
                    return index;
                }
                ++index;

                currentStop = TransportLine.GetNextStop(currentStop);

                if (index >= 32768)
                {
                    break;
                }
            }

            return 0;
        }

        /**
         * Sets the player's camera on the position of the given stop ID.
         */
        public static void SetCameraOnStop(ushort stopId)
        {
            InstanceID instanceId = InstanceID.Empty;
            instanceId.NetNode = stopId;

            var stopPosition = GetStopPosition(stopId);

            ToolsModifierControl.cameraController.SetTarget(instanceId, stopPosition, false);
        }

        /**
         * Is the citizen within the given stop ID's radius?
         *
         * @param citizenId The citizen ID
         * @param stopId The stop ID
         * @param range The range to check
         * @return `true` if the citizen is within the stop's radius, `false` otherwise
         */
        public static bool IsCitizenInRangeOfStop(ushort citizenId, ushort stopId, double range)
        {
            CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[(int)citizenId];

            Vector3 stopPosition = Bridge.GetStopPosition(stopId);

            return Vector3.SqrMagnitude((Vector3)citizen.m_targetPos - stopPosition) < (range * range);
        }

        /**
         *  Get the closest destination stop for a citizen, given their origin stop
         *  
         *  most of this is ripped from TransportArriveAtTarget
         */
        public static ushort GetDestinationStopId(ushort originalStopId, CitizenInstance citizen)
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

        /**
         * Gets the transport line ID for the given stop ID.
         */
        public static ushort GetStopTransportLineId(ushort stopId)
        {
            return GetStopNode(stopId).m_transportLine;
        }

        /**
         * Gets the color for the transport line that the given stop ID is part of.
         */
        public static Color32 GetStopLineColor(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].m_color;
        }

        /**
         * Gets the name for the transport line that the given stop ID is part of.
         */
        public static string GetStopLineName(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.GetLineName(transportLineId);
        }

        /**
         * Gets the position of the given building ID.
         */
        public static Vector3 GetBuildingPosition(ushort buildingId)
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].m_position;
        }
    }
}
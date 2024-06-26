﻿using ColossalFramework;
using CommuterDestination.Core.Bridge;
using CommuterDestination.Core.Citizen;
using System.Collections.Generic;
using UnityEngine;

namespace CommuterDestination.CS1.Core
{
    /// <summary>
    /// The <c>Bridge</c> class is responsible for interactions with the Cities: Skylines game engine
    /// 
    /// This keeps a clear separation between the mod functionality and the hackish code that is sometimes required to work with the game
    /// </summary>
    public class Bridge : IGameBridge
    {
        /// <summary>
        /// Gets the {@link NetNode} for the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the NetNode</returns>
        private NetNode GetStopNode(ushort stopId)
        {
            return Singleton<NetManager>.instance.m_nodes.m_buffer[stopId];
        }

        /// <summary>
        /// Gets the position for the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the world position of the stop</returns>
        public Vector3 GetStopPosition(ushort stopId)
        {
            return GetStopNode(stopId).m_position;
        }

        /// <summary>
        /// Gets the count of passengers currently waiting at the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the number of passengers</returns>
        public int GetStopPassengerCount(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].CalculatePassengerCount(stopId);
        }

        /// <summary>
        /// Gets the index of the stop within the line
        /// </summary>
        /// <remarks>the first stop is 1, the second is 2, etc</remarks>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the index of the stop</returns>
        public int GetStopIndex(ushort stopId)
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

                currentStop = GetNextStop(currentStop);

                if (index >= 32768)
                {
                    break;
                }
            }

            return 0;
        }

        /// <summary>
        /// Sets the player's camera on the position of the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        public void SetCameraOnStop(ushort stopId)
        {
            InstanceID instanceId = InstanceID.Empty;
            instanceId.NetNode = stopId;

            var stopPosition = GetStopPosition(stopId);

            ToolsModifierControl.cameraController.SetTarget(instanceId, stopPosition, false);
        }

        /// <summary>
        /// Is the citizen within range of the given stop ID's?
        /// </summary>
        /// <param name="citizenId">the citizen ID</param>
        /// <param name="stopId">the stop ID</param>
        /// <param name="range">the range to check</param>
        /// <returns>`true` if the citizen is within the stop's radius, `false` otherwise</returns>
        public bool IsCitizenInRangeOfStop(ushort citizenId, ushort stopId, double range)
        {
            CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenId];

            Vector3 stopPosition = GetStopPosition(stopId);

            return Vector3.SqrMagnitude((Vector3)citizen.m_targetPos - stopPosition) < range * range;
        }

        /// <summary>
        /// Gets the ID of the previous stop on the line
        /// </summary>
        /// <param name="stopId">the stop ID to look from</param>
        /// <returns>the ID of the previous stop</returns>
        public ushort GetPrevStop(ushort stopId)
        {
            return TransportLine.GetPrevStop(stopId);
        }

        /// <summary>
        /// Gets the ID of the next stop on the line
        /// </summary>
        /// <param name="stopId">the stop ID to look from</param>
        /// <returns>the ID of the next stop</returns>
        public ushort GetNextStop(ushort stopId)
        {
            return TransportLine.GetNextStop(stopId);
        }

        /// <summary>
        /// Get the transit range of a given stop.
        /// </summary>
        /// <remarks>
        /// These values are taken from the LoadPassengers game methods.
        /// </remarks>
        /// <param name="stopId">the stop to get the transit range for</param>
        /// <returns>the transit range</returns>
        public float GetStopRange(ushort stopId)
        {
            // TODO take transit type into account

            // this should be 32 for buses, cable cars, 
            //          64 for trains, ships, blimps, ferries, helicopters, planes, trams, trolleybuses
            return 64f;
        }

        /// <summary>
        /// Get the closest destination stop for a citizen, given their origin stop<br />
        /// i.e. the stop at which they will get off the transit line after getting on at the origin stop
        /// </summary>
        /// <remarks>most of this is ripped from TransportArriveAtTarget</remarks>
        /// <param name="originalStopId">the citizen's origin stop</param>
        /// <param name="citizenId">the citizen's id</param>
        /// <returns>the stop ID</returns>
        public ushort GetDestinationStopId(ushort originalStopId, ushort citizenId)
        {
            CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenId];
            ushort currentStop = GetNextStop(originalStopId);

            while (true)
            {
                var nextStop = GetNextStop(currentStop);

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

        private bool StopIsDestination(ushort currentStop, ushort nextStop, CitizenInstance citizenData)
        {
            var currentPosition = GetStopPosition(currentStop);
            var nextPosition = GetStopPosition(nextStop);

            PathManager pathManager = Singleton<PathManager>.instance;
            NetManager netManager = Singleton<NetManager>.instance;

            if ((citizenData.m_flags & CitizenInstance.Flags.OnTour) == CitizenInstance.Flags.OnTour)
            {
                if ((citizenData.m_flags & CitizenInstance.Flags.TargetIsNode) == CitizenInstance.Flags.TargetIsNode)
                {
                    ushort targetStop = citizenData.m_targetBuilding;
                    if (targetStop != 0 && (double)Vector3.SqrMagnitude(netManager.m_nodes.m_buffer[targetStop].m_position - currentPosition) < 4.0)
                    {
                        ushort stopAfterTarget = GetNextStop(targetStop);
                        if (stopAfterTarget != 0 && (double)Vector3.SqrMagnitude(netManager.m_nodes.m_buffer[stopAfterTarget].m_position - nextPosition) < 4.0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (citizenData.m_path != 0U)
            {
                citizenData.m_pathPositionIndex += 2;
                if (citizenData.m_pathPositionIndex >> 1 >= pathManager.m_pathUnits.m_buffer[(int)citizenData.m_path].m_positionCount)
                {
                    pathManager.ReleaseFirstUnit(ref citizenData.m_path);
                    citizenData.m_pathPositionIndex = 0;
                }
            }

            if (citizenData.m_path != 0U)
            {
                PathUnit.Position position;
                if (pathManager.m_pathUnits.m_buffer[(int)citizenData.m_path].GetPosition(citizenData.m_pathPositionIndex >> 1, out position))
                {
                    citizenData.m_lastPathOffset = position.m_offset;

                    uint laneId = PathManager.GetLaneID(position);
                    if ((double)Vector3.SqrMagnitude(netManager.m_lanes.m_buffer[(int)laneId].CalculatePosition(citizenData.m_lastPathOffset * 0.003921569f) - nextPosition) < 4.0)
                    {
                        //if (!forceUnload)
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the transport line ID for the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the transport line ID of the stop</returns>
        public ushort GetStopTransportLineId(ushort stopId)
        {
            return GetStopNode(stopId).m_transportLine;
        }

        /// <summary>
        /// Gets the color for the transport line that the given stop ID is part of.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the color of the stop's transport line</returns>
        public Color32 GetStopLineColor(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].m_color;
        }

        /// <summary>
        /// Gets the name for the transport line that the given stop ID is part of.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the name of the stop's transport line</returns>
        public string GetStopLineName(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.GetLineName(transportLineId);
        }

        /// <summary>
        /// Gets the position of the given building ID.
        /// </summary>
        /// <param name="buildingId">the building ID to look up</param>
        /// <returns>the position of the building</returns>
        public Vector3 GetBuildingPosition(ushort buildingId)
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].m_position;
        }

        /// <remarks>This logic is taken from LoadPassengers in the game logic (e.g. in BusAI)</remarks>
        public IEnumerable<CitizenDestination> GetCitizenDestinations(ushort stopId)
        {
            List<CitizenDestination> citizenDestinations = new List<CitizenDestination>();

            var transitRange = GetStopRange(stopId);

            Vector3 stopPosition = GetStopPosition(stopId);

            int LOWER_BOUND_X = Mathf.Max((int)((stopPosition.x - transitRange) / 8.0 + 1080.0), 0);
            int UPPER_BOUND_X = Mathf.Min((int)((stopPosition.x + transitRange) / 8.0 + 1080.0), 2159);
            int LOWER_BOUND_Z = Mathf.Max((int)((stopPosition.z - transitRange) / 8.0 + 1080.0), 0);
            int UPPER_BOUND_Z = Mathf.Min((int)((stopPosition.z + transitRange) / 8.0 + 1080.0), 2159);

            for (int z = LOWER_BOUND_Z; z <= UPPER_BOUND_Z; ++z)
            {
                for (int x = LOWER_BOUND_X; x <= UPPER_BOUND_X; ++x)
                {
                    ushort citizenInstanceId = Singleton<CitizenManager>.instance.m_citizenGrid[z * 2160 + x];
                    while (citizenInstanceId != 0)
                    {
                        CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[citizenInstanceId];
                        ushort nextGridInstance = citizen.m_nextGridInstance;

                        if (IsCitizenAtStop(ref citizen, citizenInstanceId, stopId, transitRange))
                        {
                            citizenDestinations.Add(new CitizenDestination
                            {
                                stopId = GetDestinationStopId(stopId, citizenInstanceId),
                                buildingId = citizen.m_targetBuilding
                            });
                        }

                        citizenInstanceId = nextGridInstance;
                    }
                }
            }

            return citizenDestinations;
        }

        /// <summary>
        /// Check if a given citizen is waiting at a given stop.
        /// </summary>
        /// <param name="citizen">the citizen to check</param>
        /// <param name="citizenInstanceId">the citizen's instance ID</param>
        /// <param name="stopId">the stop to check</param>
        /// <param name="stopRange">the range of the stop</param>
        /// <returns>`true` if the citizen is waiting at the stop, false otherwise</returns>
        private bool IsCitizenAtStop(ref CitizenInstance citizen, ushort citizenInstanceId, ushort stopId, float stopRange)
        {
            var citizenIsAtStop = IsCitizenInRangeOfStop(citizenInstanceId, stopId, (double)stopRange);

            // If the citizen is not within range, no need to check further
            if (!citizenIsAtStop)
            {
                return false;
            }

            ushort nextStop = GetNextStop(stopId);
            Vector3 stopPosition = GetStopPosition(stopId);
            Vector3 nextStopPosition = GetStopPosition(nextStop);

            return 
                (citizen.m_flags & CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None
                && citizen.Info.m_citizenAI.TransportArriveAtSource(citizenInstanceId, ref citizen, stopPosition, nextStopPosition)
            ;
        }
    }
}
using ColossalFramework;
using CSLShowCommuterDestination.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSLShowCommuterDestination.Game
{
    public class DestinationGraphGenerator
    {
        public static DestinationGraph GenerateGraph(ushort originalStopId)
        {
            // this method is a bit of a clone of LoadPassengers from BusAI

            ushort nextStop = TransportLine.GetNextStop(originalStopId);

            // this should be 32 for buses, cable cars, 
            //          64 for trains, ships, blimps, ferries, helicopters, planes, trams, trolleybuses
            float transitRange = 64f;

            var CITIZEN_AT_STOP_RANGE = (double)transitRange * (double)transitRange;

            Vector3 stopPosition = Bridge.GetStopPosition(originalStopId);
            Vector3 nextStopPosition = Bridge.GetStopPosition(nextStop);

            int LOWER_BOUND_X = Mathf.Max((int)(((double)stopPosition.x - (double)transitRange) / 8.0 + 1080.0), 0);
            int LOWER_BOUND_Z = Mathf.Max((int)(((double)stopPosition.z - (double)transitRange) / 8.0 + 1080.0), 0);
            int UPPER_BOUND_X = Mathf.Min((int)(((double)stopPosition.x + (double)transitRange) / 8.0 + 1080.0), 2159);
            int UPPER_BOUND_Z = Mathf.Min((int)(((double)stopPosition.z + (double)transitRange) / 8.0 + 1080.0), 2159);

            // lookup stops by id for improved performance
            Dictionary<ushort, DestinationGraphStop> stops = new Dictionary<ushort, DestinationGraphStop>();

            for (int z = LOWER_BOUND_Z; z <= UPPER_BOUND_Z; ++z)
            {
                for (int x = LOWER_BOUND_X; x <= UPPER_BOUND_X; ++x)
                {
                    ushort citizenInstanceId = Singleton<CitizenManager>.instance.m_citizenGrid[z * 2160 + x];
                    while (citizenInstanceId != (ushort)0)
                    {
                        CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[(int)citizenInstanceId];

                        ushort nextGridInstance = citizen.m_nextGridInstance;

                        var citizenIsAtStop = CitizenIsAtStop(ref citizen, citizenInstanceId, originalStopId, CITIZEN_AT_STOP_RANGE);

                        if (citizenIsAtStop)
                        {
                            var destinationStopId = Bridge.GetDestinationStopId(originalStopId, citizen);

                            if (!stops.ContainsKey(destinationStopId))
                            {
                                stops.Add(
                                    destinationStopId,
                                    new DestinationGraphStop(destinationStopId)
                                );
                            }

                            stops[destinationStopId].AddJourney(citizen.m_targetBuilding);
                        }

                        citizenInstanceId = nextGridInstance;
                    }
                }
            }

            return new DestinationGraph(stops.Select(j => j.Value));
        }

        /**
         * Check if a given citizen is waiting at a given stop.
         * 
         * @param citizen The citizen to check
         * @param citizenInstanceId The citizen instance ID
         * @param stopId The stop to check
         * @param range The range to check
         * 
         * @return true if the citizen is waiting at the stop
         */
        private static bool CitizenIsAtStop(ref CitizenInstance citizen, ushort citizenInstanceId, ushort stopId, double range)
        {
            var citizenIsAtStop = Bridge.IsCitizenInRangeOfStop(citizenInstanceId, stopId, range);

            // If the citizen is not within range, no need to check further
            if (!citizenIsAtStop)
            {
                return false;
            }

            ushort nextStop = TransportLine.GetNextStop(stopId);
            Vector3 stopPosition = Bridge.GetStopPosition(stopId);
            Vector3 nextStopPosition = Bridge.GetStopPosition(nextStop);

            return (
                (citizen.m_flags & CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None
                && citizen.Info.m_citizenAI.TransportArriveAtSource(citizenInstanceId, ref citizen, stopPosition, nextStopPosition)
            );
        }
    }
}
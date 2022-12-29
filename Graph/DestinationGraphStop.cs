using CSLShowCommuterDestination.Game;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    /**
     * A single stop on a transport line.
     *
     * Tracks the buildings that passengers are travelling to after getting off the
     * transport line at this stop.
     */
    public class DestinationGraphStop
    {
        /**
         * The position of the stop
         */
        public readonly Vector3 position;

        private readonly Dictionary<ushort, DestinationGraphJourney> journeys = new Dictionary<ushort, DestinationGraphJourney>();
        private readonly ushort stopId;

        public DestinationGraphStop(ushort stopId)
        {
            this.stopId = stopId;
            this.position = Bridge.GetStopPosition(stopId);
        }

        /**
         * Track a passenger's destination building from this stop.
         *
         * @param buildingId The building ID that the passenger is travelling to.
         */
        public void AddJourney(ushort buildingId)
        {
            if (journeys.ContainsKey(buildingId))
            {
                journeys[buildingId].IncreasePopularity();
            }
            else
            {
                var journey = new DestinationGraphJourney(this, buildingId, Bridge.GetBuildingPosition(buildingId));

                journeys.Add(buildingId, journey);
            }
        }

        /**
         * Get the list of {@link DestinationJourney} serviced by this stop
         */
        public IEnumerable<DestinationGraphJourney> GetJourneys()
        {
            return journeys.Values;
        }
    }
}
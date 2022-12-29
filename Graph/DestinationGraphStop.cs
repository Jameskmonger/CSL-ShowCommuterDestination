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

        /**
         * The ID of this stop.
         */
        private readonly ushort stopId;

        /**
         * A list of journeys from this stop.
         */
        private readonly Dictionary<ushort, DestinationGraphJourney> journeys = new Dictionary<ushort, DestinationGraphJourney>();

        public DestinationGraphStop(ushort stopId)
        {
            this.stopId = stopId;
            this.position = Bridge.GetStopPosition(stopId);
        }

        /**
         * Increase the popularity of a given building ID from this stop.
         * 
         * This will create a new {@link DestinationGraphJourney} if one does not already exist
         * for the given building ID.
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
         * Get the list of {@link DestinationJourney} from this stop
         */
        public IEnumerable<DestinationGraphJourney> GetJourneys()
        {
            return journeys.Values;
        }
    }
}
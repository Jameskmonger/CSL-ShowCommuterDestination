using CSLShowCommuterDestination.Game;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    /// <summary>
    /// A single stop on a transport line.<br/>
    /// 
    /// Tracks the buildings that passengers are travelling to after getting off the
    /// transport line at this stop.
    /// </summary>
    public class DestinationGraphStop
    {
        /// <summary>
        /// The position of the stop
        /// </summary>
        public readonly Vector3 position;

        /// <summary>
        /// The ID of this stop.
        /// </summary>
        private readonly ushort stopId;

        /// <summary>
        /// A list of journeys from this stop.
        /// </summary>
        private readonly Dictionary<ushort, DestinationGraphJourney> journeys = new Dictionary<ushort, DestinationGraphJourney>();

        public DestinationGraphStop(ushort stopId)
        {
            this.stopId = stopId;
            this.position = Bridge.GetStopPosition(stopId);
        }

        /// <summary>
        /// Increase the popularity of a given building ID from this stop.<br/>
        /// 
        /// This will create a new {@link DestinationGraphJourney} if one does not already exist
        /// for the given building ID.
        /// </summary>
        /// <param name="buildingId">the building ID that the passenger is travelling to</param>
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

        /// <summary>
        /// Get the list of {@link DestinationJourney} from this stop
        /// </summary>
        /// <returns>the journeys</returns>
        public IEnumerable<DestinationGraphJourney> GetJourneys()
        {
            return journeys.Values;
        }
    }
}
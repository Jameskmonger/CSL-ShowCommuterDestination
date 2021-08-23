using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    public class DestinationStop
    {
        private readonly Dictionary<ushort, DestinationJourney> journeys = new Dictionary<ushort, DestinationJourney>();

        private readonly ushort stopId;
        public readonly Vector3 position;

        public DestinationStop(ushort stopId, Vector3 position)
        {
            this.stopId = stopId;
            this.position = position;
        }

        public void AddJourney(ushort buildingId, Vector3 position)
        {
            if (journeys.ContainsKey(buildingId))
            {
                journeys[buildingId].IncreasePopularity();
            } else
            {
                var journey = new DestinationJourney(this, buildingId, position);

                journeys.Add(buildingId, journey);
            }
        }

        public ushort GetStopId()
        {
            return stopId;
        }

        public IEnumerable<DestinationJourney> GetJourneys()
        {
            return journeys.Values;
        }
    }
}

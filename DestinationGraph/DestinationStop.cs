using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    public class DestinationStop
    {
        private Dictionary<ushort, int> buildingPopularities = new Dictionary<ushort, int>();
        private Dictionary<ushort, Vector3> buildingPositions = new Dictionary<ushort, Vector3>();
        private ushort stopId;
        private Vector3 position;

        public DestinationStop(ushort stopId, Vector3 position)
        {
            this.stopId = stopId;
            this.position = position;
        }

        public void AddJourney(ushort buildingId, Vector3 position)
        {
            if (buildingPopularities.ContainsKey(buildingId))
            {
                int previous = buildingPopularities[buildingId];

                buildingPopularities.Remove(buildingId);

                buildingPopularities.Add(buildingId, previous + 1);
            }
            else
            {
                buildingPopularities.Add(buildingId, 1);
                buildingPositions.Add(buildingId, position);
            }
        }

        public ushort GetStopId()
        {
            return stopId;
        }

        public IEnumerable<DestinationJourney> GetJourneys()
        {
            return buildingPopularities.Select(building =>
                new DestinationJourney(stopId, building.Key, building.Value, position, buildingPositions[building.Key])
            );
        }
    }
}

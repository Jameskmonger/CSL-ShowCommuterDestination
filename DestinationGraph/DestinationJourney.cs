using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    public class DestinationJourney
    {
        public ushort stopId;
        public ushort buildingId;
        public int popularity;

        public Vector3 origin;
        public Vector3 destination;

        public DestinationJourney(ushort stopId, ushort buildingId, int popularity, Vector3 origin, Vector3 destination)
        {
            this.stopId = stopId;
            this.buildingId = buildingId;
            this.popularity = popularity;
            this.origin = origin;
            this.destination = destination;
        }
    }
}

using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    public class DestinationJourney
    {
        public DestinationStop origin;
        public ushort destinationId;
        public Vector3 destination;
        public int popularity = 1;

        public DestinationJourney(DestinationStop origin, ushort destinationId, Vector3 destination)
        {
            this.origin = origin;
            this.destinationId = destinationId;
            this.destination = destination;
        }

        public void IncreasePopularity()
        {
            popularity++;
        }
    }
}

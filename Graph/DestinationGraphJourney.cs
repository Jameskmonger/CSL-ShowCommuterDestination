using UnityEngine;

namespace CSLShowCommuterDestination.Graph
{
    /**
     * This class represents a journey between a given stop and a given destination.
     * 
     * For instance, if 100 citizens travel from "Maple Lane Bus Stop" to "High Cliff University",
     * there will be 1 DestinationGraphJourney object with a `popularity` of 100.
     */
    public class DestinationGraphJourney
    {
        /**
         * The {@link DestinationGraphStop} that the commuters got off the line at.
         */
        public DestinationGraphStop origin;

        /**
         * The building ID for the destination
         */
        public ushort destinationId;

        /**
         * The destination position
         */
        public Vector3 destination;

        /**
         * The number of commuters travelling from the origin to the destination
         */
        public int popularity = 1;

        public DestinationGraphJourney(DestinationGraphStop origin, ushort destinationId, Vector3 destination)
        {
            this.origin = origin;
            this.destinationId = destinationId;
            this.destination = destination;
        }

        /**
         * Add one commuter to the popularity count
         */
        public void IncreasePopularity()
        {
            popularity++;
        }
    }
}
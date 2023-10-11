using UnityEngine;

namespace CommuterDestination.Core.Graph
{
    /// <summary>
    /// This class represents a journey between a given stop and a given destination.    /// 
    /// </summary>
    /// <remarks>
    /// For instance, if 100 citizens travel from "Maple Lane Bus Stop" to "High Cliff University",
    /// there will be 1 DestinationGraphJourney object with a `popularity` of 100.
    /// </remarks>
    public class DestinationGraphJourney
    {
        /// <summary>
        /// The {@link DestinationGraphStop} that the commuters got off the line at.
        /// </summary>
        public DestinationGraphStop origin;

        /// <summary>
        /// The building ID for the destination
        /// </summary>
        public ushort destinationId;

        /// <summary>
        /// The destination position
        /// </summary>
        public Vector3 destination;

        /// <summary>
        /// The number of commuters travelling from the origin to the destination
        /// </summary>
        public int popularity = 1;

        public DestinationGraphJourney(DestinationGraphStop origin, ushort destinationId, Vector3 destination)
        {
            this.origin = origin;
            this.destinationId = destinationId;
            this.destination = destination;
        }

        /// <summary>
        /// Add one commuter to the popularity count
        /// </summary>
        public void IncreasePopularity()
        {
            popularity++;
        }
    }
}
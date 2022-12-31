using System.Collections.Generic;

namespace CSLShowCommuterDestination.Graph
{
    /// <summary>
    /// A graph of destinations from a series of transport line stops.
    /// </summary>
    public class DestinationGraph
    {
        /// <summary>
        /// The stops on the transport line
        /// </summary>
        public readonly IEnumerable<DestinationGraphStop> stops;

        public DestinationGraph(IEnumerable<DestinationGraphStop> stops)
        {
            this.stops = stops;
        }
    }
}
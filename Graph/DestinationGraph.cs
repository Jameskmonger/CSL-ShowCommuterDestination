using System.Collections.Generic;

namespace CSLShowCommuterDestination.Graph
{
    /**
     * A graph of destinations from a series of transport line stops.
     */
    public class DestinationGraph
    {
        /**
         * The stops on the transport line
         */
        public readonly IEnumerable<DestinationGraphStop> stops;

        public DestinationGraph(IEnumerable<DestinationGraphStop> stops)
        {
            this.stops = stops;
        }
    }
}
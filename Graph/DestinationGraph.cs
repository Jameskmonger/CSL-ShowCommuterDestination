using System.Collections.Generic;

namespace CSLShowCommuterDestination.Graph
{
    public class DestinationGraph
    {
        public readonly IEnumerable<DestinationStop> stops;

        public DestinationGraph(IEnumerable<DestinationStop> stops)
        {
            this.stops = stops;
        }
    }
}

using CommuterDestination.Core.Bridge;
using System.Collections.Generic;
using System.Linq;

namespace CommuterDestination.Core.Graph
{
    /// <summary>
    /// This class is responsible for generating a {@link DestinationGraph} for a given stop.
    /// </summary>
    public class DestinationGraphGenerator
    {
        /// <summary>
        /// Generate a {@link DestinationGraph} for a given stop.
        /// </summary>
        /// <param name="stopId">the origin stop</param>
        /// <returns>the generated graph</returns>
        public static DestinationGraph GenerateGraph(ushort stopId)
        {
            Dictionary<ushort, DestinationGraphStop> stops = new Dictionary<ushort, DestinationGraphStop>();

            var destinations = GameBridge.Instance.GetCitizenDestinations(stopId);

            foreach (var destination in destinations)
            {
                if (!stops.ContainsKey(destination.stopId))
                {
                    stops.Add(
                        destination.stopId,
                        new DestinationGraphStop(destination.stopId)
                    );
                }

                stops[destination.stopId].AddJourney(destination.buildingId);
            }

            return new DestinationGraph(stops.Select(j => j.Value));
        }
    }
}
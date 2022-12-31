using UnityEngine;
using CSLShowCommuterDestination.Graph;

namespace CSLShowCommuterDestination.Content.Renderer
{
    /// <summary>
    /// Renders a {@link DestinationGraph} using {@link Notification}s.
    /// 
    /// This is the "classic" behaviour - using the "old man" icon to show destinations.
    /// </summary>
    internal class NotificationDestinationGraphRenderer : IDestinationGraphRenderer
    {
        /// <summary>
        /// How high above the destination building should the notification be rendered?
        /// </summary>
        private static Vector3 HEIGHT_OFFSET = new Vector3(0, 50f, 0);

        /// <summary>
        /// Use the "Major" variant of the "Too Long" problem (red walking man)
        /// </summary>
        private Notification.ProblemStruct notification = new Notification.ProblemStruct(Notification.Problem1.TooLong | Notification.Problem1.MajorProblem);

        /// <summary>
        /// Renders the given <seealso cref="DestinationGraph"/> to the screen.
        /// </summary>
        /// <param name="cameraInfo">the camera</param>
        /// <param name="graph">the graph to render</param>
        public void Render(RenderManager.CameraInfo cameraInfo, DestinationGraph graph)
        {
            // Iterate through each stop in the graph and render each of the journeys from that stop.
            foreach (var stop in graph.stops)
            {
                foreach (var journey in stop.GetJourneys())
                {
                    Notification.RenderInstance(
                        cameraInfo, 
                        this.notification,
                        journey.destination + HEIGHT_OFFSET,
                        (float)(1 + (journey.popularity / 5))
                    );
                }
            }
        }
    }
}

﻿using UnityEngine;
using CSLShowCommuterDestination.Graph;

namespace CSLShowCommuterDestination.Content.Renderer
{
    /**
     * Renders a {@link DestinationGraph} using {@link Notification}s.
     * 
     * This is the "classic" behaviour - using the "old man" icon to show destinations.
     */
    internal class NotificationDestinationGraphRenderer : IDestinationGraphRenderer
    {
        private static Vector3 HEIGHT_OFFSET = new Vector3(0, 50f, 0);

        /**
         * Use the "Major" variant of the "Too Long" problem (red walking man)
         */
        private Notification.ProblemStruct notification = new Notification.ProblemStruct(Notification.Problem1.TooLong | Notification.Problem1.MajorProblem);

        public void Render(RenderManager.CameraInfo cameraInfo, DestinationGraph graph)
        {
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

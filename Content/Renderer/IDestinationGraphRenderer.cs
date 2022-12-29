using CSLShowCommuterDestination.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLShowCommuterDestination.Content.Renderer
{
    /**
     * This interface represents a renderer for a {@link DestinationGraph}.
     */
    internal interface IDestinationGraphRenderer
    {
        /**
         * Renders the given {@link DestinationGraph} to the screen.
         */
        void Render(RenderManager.CameraInfo cameraInfo, DestinationGraph graph);
    }
}

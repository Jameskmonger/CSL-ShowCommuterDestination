using CSLShowCommuterDestination.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLShowCommuterDestination.Content.Renderer
{
    internal interface IDestinationGraphRenderer
    {
        void Render(RenderManager.CameraInfo cameraInfo, DestinationGraph graph);
    }
}

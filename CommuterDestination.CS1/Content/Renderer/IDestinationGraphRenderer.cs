using CommuterDestination.Core.Graph;

namespace CommuterDestination.CS1.Content.Renderer
{
    /// <summary>
    /// This interface represents a renderer for a <seealso cref="DestinationGraph"/>
    /// </summary>
    internal interface IDestinationGraphRenderer
    {
        /// <summary>
        /// Renders the given <seealso cref="DestinationGraph"/> to the screen.
        /// </summary>
        /// <param name="cameraInfo">the camera</param>
        /// <param name="graph">the graph to render</param>
        void Render(RenderManager.CameraInfo cameraInfo, DestinationGraph graph);
    }
}

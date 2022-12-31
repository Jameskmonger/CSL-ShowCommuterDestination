using UnityEngine;
using CSLShowCommuterDestination.Content.Renderer;
using CSLShowCommuterDestination.UI;

namespace CSLShowCommuterDestination.Content
{
    /// <summary>
    /// Responsible for rendering the <seealso cref="DestinationGraph"/> when the user has the
    /// mod panel <seealso cref="StopDestinationInfoPanel"/> open.
    /// </summary>
    public class DestinationDisplayManager : SimulationManagerBase<DestinationDisplayManager, MonoBehaviour>, IRenderableManager
    {
        /// <summary>
        /// A renderer for the default, red, "old man" notification icon.
        /// </summary>
        private readonly IDestinationGraphRenderer notificationRenderer = new NotificationDestinationGraphRenderer();

        protected override void BeginOverlayImpl(RenderManager.CameraInfo cameraInfo)
        {
            if (!StopDestinationInfoPanel.instance || !StopDestinationInfoPanel.instance.isVisible)
            {
                return;
            }

            if (StopDestinationInfoPanel.instance.DestinationGraph == null)
            {
                return;
            }

            this.GetRenderer().Render(
                cameraInfo,
                StopDestinationInfoPanel.instance.DestinationGraph
            );
        }

        private IDestinationGraphRenderer GetRenderer()
        {
            // TODO wire in new render system here

            return notificationRenderer;
        }
    }
}

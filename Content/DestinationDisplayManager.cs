using UnityEngine;
using CSLShowCommuterDestination.Content.Renderer;

namespace CSLShowCommuterDestination
{
    /**
     * Responsible for rendering the {@link DestinationGraph} when the user has the
     * mod panel open.
     */
    public class DestinationDisplayManager : SimulationManagerBase<DestinationDisplayManager, MonoBehaviour>, IRenderableManager
    {
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

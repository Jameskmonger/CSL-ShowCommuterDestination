using ColossalFramework;
using UnityEngine;

namespace CSLShowCommuterDestination
{
    public class DestinationDisplayManager : SimulationManagerBase<DestinationDisplayManager, MonoBehaviour>, IRenderableManager
    {
        public Notification.ProblemStruct m_Notification = new Notification.ProblemStruct(Notification.Problem1.TooLong | Notification.Problem1.MajorProblem);

        protected override void BeginOverlayImpl(RenderManager.CameraInfo cameraInfo)
        {
            if (!StopDestinationInfoPanel.instance || !StopDestinationInfoPanel.instance.isVisible)
            {
                return;
            }

            //foreach (var popularity in StopDestinationInfoPanel.instance.m_BuildingPopularities)
            //{
            //    // get the position of the building
            //    Vector3 position = Singleton<BuildingManager>.instance.m_buildings.m_buffer[popularity.Key].m_position;

            //    // raise the icon in the air, this should probably use building height
            //    position.y += 50f;

            //    // render the notification
            //    Notification.RenderInstance(cameraInfo, this.m_Notification, position, (float)(1 + (popularity.Value / 5)));
            //}
        }
    }
}

using CSLShowCommuterDestination.UI;
using HarmonyLib;
using System;
using System.Reflection;

namespace CSLShowCommuterDestination.Integrations.IPT2
{
    /// <summary>
    /// This patch listens to the "Previous Stop" and "Next Stop" buttons in the IPT2 stop info panel.
    /// 
    /// When the user clicks on one of these buttons, Commuter Destination will update to show the
    /// destinations for the new stop.
    /// </summary>
    [HarmonyPatch]
    public static class IPT2StopNavigationPatch
    {
        /// <summary>
        /// The full type name of the IPT2 panel class.
        /// </summary>
        private const string IPT2PanelTypeName = IPT2Integration.ASSEMBLY_NAME + ".PublicTransportStopWorldInfoPanel, " + IPT2Integration.ASSEMBLY_NAME;

        /// <summary>
        /// This will return `false` if the IPT2 class can't be found, which allows us to
        /// skip patching if IPT2 isn't present.
        /// 
        /// https://harmony.pardeike.net/articles/patching-auxilary.html#prepare
        /// </summary>
        /// <returns>true if IPT2 class is found, false otherwise</returns>
        /// <remarks>TODO we should maybe be taking `MethodBase original` in as a parameter here</remarks>
        /// <remarks>TODO can/should this tie into ModIntegrations instead? That already checks assembly presence</remarks>
        public static bool Prepare() => Type.GetType(IPT2PanelTypeName) != null;

        /// <summary>
        /// The target method of the patch - `PublicTransportStopWorldInfoPanel.ChangeInstanceID`
        /// </summary>
        /// <returns>The ChangeInstanceID method</returns>
        /// <remarks>This method is private, so we should be aware that it might break in future</remarks>
        public static MethodBase TargetMethod() => Type.GetType(IPT2PanelTypeName)?.GetMethod("ChangeInstanceID", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// When ChangeInstanceID is called, we need to call `Show` on our Commuter Destination info panel with the new stop ID.
        /// </summary>
        /// <param name="oldID">The old stop ID, currently unused</param>
        /// <param name="newID">The stop ID being changed to</param>
        public static void Postfix(
            InstanceID oldID, InstanceID newID
        )
        {
            // TODO this shouldn't be referring to the UI panel directly - bad separation of concerns
            StopDestinationInfoPanel.instance.Show(newID.NetNode);
        }
    }
}

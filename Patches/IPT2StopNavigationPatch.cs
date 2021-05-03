using HarmonyLib;
using System;
using System.Reflection;

namespace CSLShowCommuterDestination.Patches
{
    [HarmonyPatch]
    public static class IPT2StopNavigationPatch
    {
        public static MethodBase TargetMethod()
        {
            return Type.GetType("ImprovedPublicTransport2.PublicTransportStopWorldInfoPanel, ImprovedPublicTransport2")?
                    .GetMethod("ChangeInstanceID", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void Postfix(
            InstanceID oldID, InstanceID newID
        )
        {
            StopDestinationInfoPanel.instance.Show(newID.NetNode);
        }
    }
}

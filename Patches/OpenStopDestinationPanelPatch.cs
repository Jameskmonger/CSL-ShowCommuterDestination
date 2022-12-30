using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using CSLShowCommuterDestination.Game.Integrations;
using HarmonyLib;
using UnityEngine;

namespace CSLShowCommuterDestination.Patches
{
    /// <summary>
    /// This patch listens to mouse clicks on the PublicTransportStopButton and opens the Commuter Destination info panel
    /// for the stop that was clicked.
    /// 
    /// There is a compatibility patch in here to account for IPT2, which also applies a patch to the same method.
    /// </summary>
    [HarmonyPatch]
    public static class OpenStopDestinationPanelPatch
    {
        /// <summary>
        /// Lists the methods that we want to patch. This allows us to patch multiple methods with the same patch.
        /// 
        /// <list type="bullet">
        ///     <item>the original method in the base game</item>
        ///     <item>the IPT2 method, if IPT2 is present</item>
        /// </list>
        /// </summary>
        /// <remarks>We need to patch the IPT2 method because it's using the "Redirection Framework" rather than Harmony.</remarks>
        /// <returns>The list of methods</returns>
        public static IEnumerable<MethodBase>TargetMethods()
        {
            // The original method in the base game
            yield return typeof(PublicTransportStopButton).GetMethod("OnMouseDown", BindingFlags.Instance | BindingFlags.NonPublic);

            // If IPT2 is present, we need to patch the IPT2 method as well
            Type iptType = Type.GetType(IPT2Integration.ASSEMBLY_NAME + ".Detour.PublicTransportStopButtonDetour, " + IPT2Integration.ASSEMBLY_NAME);
            if (iptType != null)
            {
                yield return iptType.GetMethod("OnMouseDown", BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

        /// <summary>
        /// Open the Commuter Destination info panel when the user clicks on a stop.
        /// </summary>
        /// <param name="__instance">The PublicTransportStopButton which was clicked (the instance being patched)</param>
        /// <param name="component">The UIComponent which was clicked</param>
        /// <param name="eventParam">Mouse event params (unused)</param>
        public static void Postfix(
            PublicTransportStopButton __instance,
            UIComponent component,
            UIMouseEventParameter eventParam
        )
        {
            ushort stopId = (ushort)(component as UIButton).objectUserData;

            Debug.Log("Opening stop destination screen for " + stopId);

            StopDestinationInfoPanel.instance.Show(stopId);
        }
    }
}

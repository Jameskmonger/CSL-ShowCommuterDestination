using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using CommuterDestination.CS1.Integrations;
using CommuterDestination.CS1.Integrations.IPT2;
using CommuterDestination.CS1.UI;
using HarmonyLib;
using UnityEngine;

namespace CommuterDestination.CS1.Patches
{
    /// <summary>
    /// This patch listens to mouse clicks on the PublicTransportStopButton and opens the Commuter Destination info panel
    /// for the stop that was clicked.
    /// </summary>
    [HarmonyPatch(typeof(PublicTransportStopButton), "OnMouseDown")]
    public static class OpenStopDestinationPanelPatch
    {
        /// <summary>
        /// Open the Commuter Destination info panel when the user clicks on a stop.
        /// </summary>
        /// <param name="__instance">The PublicTransportStopButton which was clicked (the instance being patched)</param>
        /// <param name="component">The UIComponent which was clicked</param>
        /// <param name="eventParam">Mouse event params (unused)</param>
        /// <remarks>Set as priority 0 so it runs before IPT2 (if present).</remarks>
        [HarmonyPrefix, HarmonyBefore(IPT2Integration.HARMONY_ID)]
        public static bool Prefix(
            PublicTransportStopButton __instance,
            UIComponent component,
            UIMouseEventParameter eventParam
        )
        {
            ushort stopId = (ushort)(component as UIButton).objectUserData;

            // TODO this probably shouldn't be referring to the UI panel directly - bad separation of concerns
            StopDestinationInfoPanel.instance.Show(stopId);

            return true;
        }
    }
}

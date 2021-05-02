using ColossalFramework.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CSLShowCommuterDestination.Patches
{
    [HarmonyPatch]
    public static class IPT2CompatabilityPatch
    {
        public static MethodBase TargetMethod()
        {
            return Type.GetType("ImprovedPublicTransport2.Detour.PublicTransportStopButtonDetour, ImprovedPublicTransport2")?
                .GetMethod("OnMouseDown", BindingFlags.Instance | BindingFlags.NonPublic);
        }

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

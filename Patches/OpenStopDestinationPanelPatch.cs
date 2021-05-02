using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using HarmonyLib;
using UnityEngine;

namespace CSLShowCommuterDestination.Patches
{
    [HarmonyPatch]
    public static class OpenStopDestinationPanelPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PublicTransportStopButton).GetMethod("OnMouseDown", BindingFlags.Instance | BindingFlags.NonPublic);
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

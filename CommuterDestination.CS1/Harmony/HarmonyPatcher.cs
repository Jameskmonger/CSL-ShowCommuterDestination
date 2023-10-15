using UnityEngine;

namespace CommuterDestination.CS1.Harmony
{
    public static class HarmonyPatcher
    {
        public static void Patch()
        {
            Debug.Log("jameskmonger.CSLShowCommuterDestination Patching");
            var harmony = new HarmonyLib.Harmony(HarmonyConstants.HARMONY_ID);
            harmony.PatchAll(typeof(HarmonyPatcher).Assembly);

            SimulationManager.RegisterManager(ColossalFramework.Singleton<DestinationDisplayManager>.instance);
        }

        public static void Unpatch()
        {
            var harmony = new HarmonyLib.Harmony(HarmonyConstants.HARMONY_ID);
            harmony.UnpatchAll(HarmonyConstants.HARMONY_ID);
            Debug.Log("jameskmonger.CSLShowCommuterDestination Reverted");
        }
    }
}

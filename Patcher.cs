using HarmonyLib;
using UnityEngine;
using CSLShowCommuterDestination.Content;

namespace CSLShowCommuterDestination {
    public static class Patcher {
        private const string kHarmonyId = "jameskmonger.CSLShowCommuterDestination";

        public static void Patch() {
            Debug.Log("jameskmonger.CSLShowCommuterDestination Patching");
            var harmony = new Harmony(kHarmonyId);
            harmony.PatchAll(typeof(Patcher).Assembly);

            SimulationManager.RegisterManager((UnityEngine.Object)ColossalFramework.Singleton<DestinationDisplayManager>.instance);
        }

        public static void Unpatch() {
            var harmony = new Harmony(kHarmonyId);
            harmony.UnpatchAll(kHarmonyId);
            Debug.Log("jameskmonger.CSLShowCommuterDestination Reverted");
        }
    }
}

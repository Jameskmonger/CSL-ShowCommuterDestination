using HarmonyLib;
using UnityEngine;

namespace CSLShowCommuterDestination {
    public static class Patcher {
        private const string kHarmonyId = "jameskmonger.CSLShowCommuterDestination";

        public static void Patch() {
            Debug.Log("jameskmonger.CSLShowCommuterDestination Patching");
            var harmony = new Harmony(kHarmonyId);
            harmony.PatchAll(typeof(Patcher).Assembly);
        }

        public static void Unpatch() {
            var harmony = new Harmony(kHarmonyId);
            harmony.UnpatchAll(kHarmonyId);
            Debug.Log("jameskmonger.CSLShowCommuterDestination Reverted");
        }
    }
}

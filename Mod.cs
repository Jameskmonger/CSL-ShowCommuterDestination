using CitiesHarmony.API;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CSLShowCommuterDestination.Content;
using ICities;
using System.Collections;
using System.Reflection;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace CSLShowCommuterDestination {
    public class Mod : LoadingExtensionBase, IUserMod
    {
        private GameObject stopDestinationInfoPanel;

        public static GameObject Prefab_DestinationGraphRenderer;
        public static GameObject Prefab_DestinationStopRenderer;
        public static GameObject Prefab_DestinationJourneyRenderer;

        public string Name => "CSL Commuter Destination";
        public string Description => "See the destination of all passengers waiting at a public transport stop.";

        private static string cachedModPath = null;

        public static string ModPath =>
            cachedModPath ?? (cachedModPath =
                PluginManager.instance.FindPluginInfo(Assembly.GetAssembly(typeof(Mod))).modPath);

        public void OnEnabled() {
            UnityEngine.Debug.Log("CSL Commuter Destination enabled");
            HarmonyHelper.DoOnHarmonyReady(Patcher.Patch);
        }

        public void OnDisabled() {
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.Unpatch();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            UIView uiView = UnityEngine.Object.FindObjectOfType<UIView>();
            if (uiView != null)
            {
                stopDestinationInfoPanel = new GameObject("StopDestinationInfoPanel");
                stopDestinationInfoPanel.transform.parent = uiView.transform;
                stopDestinationInfoPanel.AddComponent<StopDestinationInfoPanel>();
            }

            var loader = new GameObject("CommuterDestinationPrefabLoader");
            loader.AddComponent<PrefabLoader>();
        }        
    }
}

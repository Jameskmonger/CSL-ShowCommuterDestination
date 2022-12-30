using CitiesHarmony.API;
using ColossalFramework.UI;
using CSLShowCommuterDestination.Game.Integrations;
using ICities;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace CSLShowCommuterDestination
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        private GameObject stopDestinationInfoPanel;

        public const string Version = "0.4.0";

        public string Name => "CSL Commuter Destination " + Version;
        
        public string Description => "See the destination of all passengers waiting at a public transport stop.";

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
            if ((UnityEngine.Object)uiView != (UnityEngine.Object)null)
            {
                this.stopDestinationInfoPanel = new GameObject("StopDestinationInfoPanel");
                this.stopDestinationInfoPanel.transform.parent = uiView.transform;
                this.stopDestinationInfoPanel.AddComponent<StopDestinationInfoPanel>();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            ModIntegrations.CheckEnabledMods();

            var group = helper.AddGroup(Name) as UIHelper;
            var panel = group.self as UIPanel;
            group.AddSpace(10);

            group = helper.AddGroup("Integrations") as UIHelper;
            panel = group.self as UIPanel;

            var ipt2Label = panel.AddUIComponent<UILabel>();
            ipt2Label.name = "integration_ipt2";
            ipt2Label.text = "Improved Public Transport 2: " + (ModIntegrations.IsIPT2Enabled() ? "Detected" : "Not detected");
        }
    }
}

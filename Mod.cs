using CitiesHarmony.API;
using ColossalFramework.UI;
using CSLShowCommuterDestination.UI;
using ICities;
using UnityEngine;

namespace CSLShowCommuterDestination
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        private GameObject stopDestinationInfoPanel;

        public const string Version = "0.4.1";

        public string Name => "CSL Commuter Destination " + Version;
        
        public string Description => "See the destination of all passengers waiting at a public transport stop.";

        /// <summary>
        /// Set up Harmony patches
        /// </summary>
        public void OnEnabled() {
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                Debug.LogError("CSL Commuter Destination requires Harmony, no Harmony installation found.");
                return;
            }

            Debug.Log("CSL Commuter Destination enabled");
            HarmonyHelper.DoOnHarmonyReady(Patcher.Patch);
        }

        /// <summary>
        /// Unpatch Harmony patches
        /// </summary>
        /// <remarks>TODO also clean up GameObjects</remarks>
        public void OnDisabled() {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.Unpatch();
            }
        }

        /// <summary>
        /// Called when the load state changes. Responsible for setting up the main mod
        /// </summary>
        /// <param name="mode">the load type</param>
        /// <remarks>TODO also clean up GameObjects</remarks>
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                Debug.LogError("CSL Commuter Destination requires Harmony, no Harmony installation found.");
                return;
            }

            UIView uiView = UnityEngine.Object.FindObjectOfType<UIView>();
            if ((UnityEngine.Object)uiView != (UnityEngine.Object)null)
            {
                this.stopDestinationInfoPanel = new GameObject("StopDestinationInfoPanel");
                this.stopDestinationInfoPanel.transform.parent = uiView.transform;
                this.stopDestinationInfoPanel.AddComponent<StopDestinationInfoPanel>();
            }
        }

        /// <summary>
        /// Called to set up the mod settings panel
        /// </summary>
        /// <param name="helper">A UI helper</param>
        public void OnSettingsUI(UIHelperBase helper)
        {
            var group = helper.AddGroup(Name) as UIHelper;
                
            group.AddSpace(10);

            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                Debug.LogError("CSL Commuter Destination requires Harmony, no Harmony installation found.");

                var panel = group.self as UIPanel;
                
                var label = panel.AddUIComponent<UILabel>();
                label.name = "CommuterDestinationNotRunningHarmony";
                label.text = "CSL Commuter Destination requires Harmony, no Harmony installation found. The mod is not running.";
                return;
            }
            
            SettingsUI.BuildPanel(group);
        }
    }
}

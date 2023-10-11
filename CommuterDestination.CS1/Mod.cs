using CitiesHarmony.API;
using ColossalFramework.UI;
using CommuterDestination.Core.Bridge;
using CommuterDestination.CS1.Game;
using CommuterDestination.CS1.UI;
using ICities;
using UnityEngine;

namespace CommuterDestination.CS1
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        private StopDestinationInfoPanel stopDestinationInfoPanel;

        public const string Version = "0.4.1";

        public string Name => "CSL Commuter Destination " + Version;

        public string Description => "See the destination of all passengers waiting at a public transport stop.";

        /// <summary>
        /// Set up Harmony patches
        /// </summary>
        public void OnEnabled()
        {
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
        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.Unpatch();
            }

            CleanUp();
        }

        /// <summary>
        /// Called when the load state changes. Responsible for setting up the main mod
        /// </summary>
        /// <param name="mode">the load type</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                Debug.LogError("CSL Commuter Destination requires Harmony, no Harmony installation found.");
                return;
            }

            CleanUp();

            if (mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario && mode != LoadMode.LoadGame)
            {
                return;
            }

            SetUp();
        }

        private void CreateBridge()
        {
            GameBridge.SetInstance(new Bridge());
        }

        /// <summary>
        /// Set up the GameObjects associated with the mod.
        /// </summary>
        private void SetUp()
        {
            CreateBridge();

            var uiView = UIView.GetAView();

            if (uiView != null)
            {
                stopDestinationInfoPanel = new GameObject("StopDestinationInfoPanel").AddComponent<StopDestinationInfoPanel>();
                stopDestinationInfoPanel.transform.parent = uiView.transform;
            }
        }

        /// <summary>
        /// Clean up all instantiated GameObjects. This is important so we avoid breakages if people load into
        /// a save after the mod has already been started.
        /// </summary>
        private void CleanUp()
        {
            if (stopDestinationInfoPanel != null)
            {
                GameObject.DestroyImmediate(stopDestinationInfoPanel.gameObject);
                stopDestinationInfoPanel = null;
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

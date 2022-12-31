﻿using ColossalFramework.UI;
using CSLShowCommuterDestination.Game.Integrations;

namespace CSLShowCommuterDestination.UI
{
    /// <summary>
    /// The settings panel for the mod.
    /// </summary>
    public class SettingsUI
    {
        /// <summary>
        /// Builds the settings panel with the given UIHelper.
        /// </summary>
        /// <param name="helper">the UIHelper to build with</param>
        public static void BuildPanel(UIHelper helper)
        {
            BuildIntegrationsSettings(helper);
        }

        /// <summary>
        /// Builds the integrations settings with the given UIHelper.
        /// </summary>
        /// <param name="helper">the UIHelper to build with</param>
        private static void BuildIntegrationsSettings(UIHelper helper)
        {
            var integrationsGroup = helper.AddGroup("Integrations") as UIHelper;
            var integrationsPanel = integrationsGroup.self as UIPanel;

            var ipt2Label = integrationsPanel.AddUIComponent<UILabel>();
            ipt2Label.name = "integration_ipt2";
            ipt2Label.text = "Improved Public Transport 2: " + (ModIntegrations.IsIPT2Enabled() ? "Detected" : "Not detected");
        }
    }
}

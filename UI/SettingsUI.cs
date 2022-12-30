using ColossalFramework.UI;
using CSLShowCommuterDestination.Game.Integrations;

namespace CSLShowCommuterDestination.UI
{
    public class SettingsUI
    {
        public static void BuildPanel(UIHelper helper)
        {
            BuildIntegrationsPanel(helper);
        }

        private static void BuildIntegrationsPanel(UIHelper helper)
        {
            var integrationsGroup = helper.AddGroup("Integrations") as UIHelper;
            var integrationsPanel = integrationsGroup.self as UIPanel;

            var ipt2Label = integrationsPanel.AddUIComponent<UILabel>();
            ipt2Label.name = "integration_ipt2";
            ipt2Label.text = "Improved Public Transport 2: " + (ModIntegrations.IsIPT2Enabled() ? "Detected" : "Not detected");
        }
    }
}

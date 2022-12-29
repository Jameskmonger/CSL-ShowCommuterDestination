using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSLShowCommuterDestination.Game.Integrations
{
    /**
     * This class handles integrations with 3rd party mods - mostly checking if they are installed.
     */
    public class ModIntegrations
    {
        private static bool isIPT2Enabled = false;

        public static void CheckEnabledMods()
        {
            var enabledMods = new HashSet<string>();
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (plugin.isEnabled) enabledMods.Add(assembly.GetName().Name.ToLower());
                }
            }

            isIPT2Enabled = enabledMods.Contains(IPT2Integration.ASSEMBLY_NAME);
        }

        /**
         * Check if Improved Public Transport 2 is currently enabled.
         * 
         * https://steamcommunity.com/sharedfiles/filedetails/?id=928128676
         * 
         * @return true if IPT2 is enabled, false otherwise.
         */
        public static bool IsIPT2Enabled()
        {
            return isIPT2Enabled;
        }
    }
}

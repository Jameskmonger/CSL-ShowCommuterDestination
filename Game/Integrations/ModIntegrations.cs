using ColossalFramework.Plugins;
using System.Collections.Generic;
using System.Reflection;

namespace CSLShowCommuterDestination.Game.Integrations
{
    /// <summary>
    /// This class handles integrations with 3rd party mods - checking if they are installed.
    /// </summary>
    public class ModIntegrations
    {
        private static bool isIPT2Enabled = false;

        /// <summary>
        /// Iterates through the currently enabled mods and checks for the presence of assemblies relating to
        /// 3rd party mods that we support integrations with.
        /// </summary>
        public static void CheckEnabledMods()
        {
            var enabledMods = new HashSet<string>();
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (plugin.isEnabled)
                    {
                        enabledMods.Add(assembly.GetName().Name.ToLower());
                    }
                }
            }

            isIPT2Enabled = enabledMods.Contains(IPT2Integration.ASSEMBLY_NAME.ToLower());
        }
        
        /// <summary>
        /// Is Improved Public Transport 2 currently enabled?
        /// 
        /// https://steamcommunity.com/sharedfiles/filedetails/?id=928128676
        /// </summary>
        /// <returns>true if IPT2 is enabled, false otherwise.</returns>
        public static bool IsIPT2Enabled()
        {
            return isIPT2Enabled;
        }
    }
}

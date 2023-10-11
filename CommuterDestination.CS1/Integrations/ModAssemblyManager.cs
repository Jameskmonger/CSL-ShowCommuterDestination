using ColossalFramework.Plugins;
using ICities;
using System.Collections.Generic;
using System.Reflection;

namespace CommuterDestination.CS1.Integrations
{
    /// <summary>
    /// Responsible for checking the presence of the assemblies of other mods we care about.
    /// </summary>
    public class ModAssemblyManager : LoadingExtensionBase
    {
        /// <summary>
        /// The manager instance, used internally to allow static access to this class as a singleton.
        /// </summary>
        private static ModAssemblyManager instance;

        private Dictionary<string, bool> cachedModAssemblies = new Dictionary<string, bool>();
        private bool gameLoaded = false;

        /// <summary>
        /// From LoadingExtensionBase. Stores the instance of the class for future singleton access.
        /// </summary>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            
            instance = this;
        }

        /// <summary>
        /// From LoadingExtensionBase. Tracks whether the game has been loaded into a playable state.
        /// </summary>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario && mode != LoadMode.LoadGame)
            {
                return;
            }

            gameLoaded = true;
        }

        /// <summary>
        /// From LoadingExtensionBase. Cleans up any state.
        /// </summary>
        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            gameLoaded = false;
            cachedModAssemblies = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Checks whether a given assembly is enabled
        /// </summary>
        /// <remarks>This method will cache the result if the game is currently in a loaded state</remarks>
        /// <param name="assemblyName">The assembly to check</param>
        /// <returns>true if the assembly is enabled, false otherwise</returns>
        public static bool IsModAssemblyEnabled(string assemblyName)
        {
            if (instance == null || !instance.gameLoaded)
            {
                return IsAssemblyEnabled(assemblyName);
            }
            
            if (instance.cachedModAssemblies.TryGetValue(assemblyName, out bool enabled))
            {
                return enabled;
            }

            enabled = IsAssemblyEnabled(assemblyName);

            instance.cachedModAssemblies[assemblyName] = enabled;

            return enabled;
        }

        /// <summary>
        /// Look up whether the given assembly name is present and enabled
        /// </summary>
        /// <param name="assemblyName">The assembly to check</param>
        /// <returns>true if the assembly is enabled, false otherwise</returns>
        private static bool IsAssemblyEnabled(string assemblyName)
        {
            var assemblyNameLower = assemblyName.ToLower();

            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (assembly.GetName().Name.ToLower() == assemblyNameLower)
                    {
                        return plugin.isEnabled;
                    }
                }
            }

            return false;
        }
    }
}

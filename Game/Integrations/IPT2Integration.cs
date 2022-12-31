using System;
using System.Reflection;
using UnityEngine;

namespace CSLShowCommuterDestination.Game.Integrations
{
    /// <summary>
    /// Integration with Improved Public Transport 2<br/>
    /// https://steamcommunity.com/sharedfiles/filedetails/?id=928128676
    /// </summary>
    public class IPT2Integration
    {
        /// <summary>
        /// The name of the IPT2 assembly
        /// </summary>
        public const string ASSEMBLY_NAME = "ImprovedPublicTransport2";

        /// <summary>
        /// Is the IPT2 integration enabled? i.e. is the IPT2 mod installed?
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                return ModAssemblyManager.IsModAssemblyEnabled(ASSEMBLY_NAME);
            }
        }

        /// <summary>
        /// Open the IPT2 stop panel for the given stop.
        /// </summary>
        /// <param name="stopId">the stop id to open</param>
        public static void ShowStopPanel(ushort stopId)
        {
            if (!IsEnabled)
            {
                return;
            }

            InstanceID instanceId = InstanceID.Empty;
            instanceId.NetNode = stopId;

            if (!InstanceManager.IsValid(instanceId))
            {
                Debug.LogWarning("Invalid instance ID for IPT2Integration.ShowStopPanel");
                return;
            }

            Type iptType = Type.GetType(ASSEMBLY_NAME + ".PublicTransportStopWorldInfoPanel, " + ASSEMBLY_NAME);

            if (iptType == null)
            {
                Debug.LogWarning("IPT2 panel type not found");
                return;
            }

            var instanceField = iptType.GetField("instance", BindingFlags.Public | BindingFlags.Static);

            if (instanceField == null)
            {
                Debug.LogWarning("'instance' field found in PublicTransportStopWorldInfoPanel");
                return;
            }

            var iptStopPanelInstance = instanceField.GetValue(null);

            if (iptStopPanelInstance == null)
            {
                Debug.LogWarning("'instance' field was null");
                return;
            }

            var showMethod = iptType.GetMethod(
                "Show",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(Vector3), typeof(InstanceID) },
                null
            );

            if (showMethod == null)
            {
                Debug.LogWarning("'Show' method not found in PublicTransportStopWorldInfoPanel");
                return;
            }
            
            var arguments = new object[] { Bridge.GetStopPosition(stopId), instanceId };
            showMethod.Invoke(iptStopPanelInstance, arguments);
        }
    }
}

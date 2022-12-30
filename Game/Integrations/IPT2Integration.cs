using System;
using System.Reflection;
using UnityEngine;

namespace CSLShowCommuterDestination.Game.Integrations
{
    /**
     * Integration with Improved Public Transport 2
     * 
     * https://steamcommunity.com/sharedfiles/filedetails/?id=928128676
     */
    public class IPT2Integration
    {
        public const string ASSEMBLY_NAME = "ImprovedPublicTransport2";

        public static void ShowStopPanel(ushort stopId)
        {
            if (!ModIntegrations.IsIPT2Enabled())
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

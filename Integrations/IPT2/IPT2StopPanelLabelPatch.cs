using ColossalFramework.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace CSLShowCommuterDestination.Integrations.IPT2
{
    /// <summary>
    /// This patch adds a small label to the IPT2 stop panel, letting the user know that Commuter Destination is running.
    /// 
    /// We do this because we hide the Commuter Destination panel if IPT2 is installed, to reduce clutter.
    /// </summary>
    [HarmonyPatch]
    public static class IPT2StopPanelLabelPatch
    {
        /// <summary>
        /// The height of the label
        /// </summary>
        private const float LabelHeight = 22f;
        
        /// <summary>
        /// The full type name of the IPT2 panel class.
        /// </summary>
        private const string IPT2PanelTypeName = IPT2Integration.ASSEMBLY_NAME + ".PublicTransportStopWorldInfoPanel, " + IPT2Integration.ASSEMBLY_NAME;
        
        /// <summary>
        /// This will return `false` if the IPT2 class can't be found, which allows us to
        /// skip patching if IPT2 isn't present.
        /// 
        /// https://harmony.pardeike.net/articles/patching-auxilary.html#prepare
        /// </summary>
        /// <returns>true if IPT2 class is found, false otherwise</returns>
        /// <remarks>TODO we should maybe be taking `MethodBase original` in as a parameter here</remarks>
        public static bool Prepare() => Type.GetType(IPT2PanelTypeName) != null;

        /// <summary>
        /// The target method of the patch - `PublicTransportStopWorldInfoPanel.SetupPanel`
        /// </summary>
        /// <returns>The SetupPanel method</returns>
        /// <remarks>This method is private, so we should be aware that it might break in future</remarks>
        public static MethodBase TargetMethod() => Type.GetType(IPT2PanelTypeName)?.GetMethod("SetupPanel", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Modify the code instructions in order to call our custom `AddCommuterDestinationLabel` method.
        /// </summary>
        /// <param name="instructions">The input instuctions</param>
        /// <returns>The modified instructions</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Find the index of the passenger count label assignment - we want to insert our label after this
            var passengerLabelAssignmentIndex = codes.FindIndex(code => code.opcode == OpCodes.Stfld && ((FieldInfo)code.operand).Name == "m_PassengerCount");
            
            var newInstructions = new List<CodeInstruction>
            {
                // use dup to copy the container from the stack
                new CodeInstruction(OpCodes.Dup),

                // call our AddCommuterDestinationLabel on the container
                new CodeInstruction(
                    OpCodes.Callvirt,
                    typeof(IPT2StopPanelLabelPatch).GetMethod(
                        nameof(AddCommuterDestinationLabel),
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
                ),
            };
            
            // add our new codes after the passenger label is assigned
            codes.InsertRange(passengerLabelAssignmentIndex + 1, newInstructions);

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Adds the Commuter Destination label to the container and adjusts the height of the container's parent to accomodate it.
        /// </summary>
        /// <param name="container">The container</param>
        private static void AddCommuterDestinationLabel(UIPanel container)
        {
            var label = CreateCommuterDestinationLabel(container);

            // adjust the height of the panel to support our new label
            container.parent.height += label.height;

            Debug.Log("Commuter Destination: Added label to IPT2 panel");
        }

        /// <summary>
        /// Creates the Commuter Destination label within the container
        /// </summary>
        /// <param name="container">The container</param>
        /// <returns>The newly inserted label container</returns>
        private static UIPanel CreateCommuterDestinationLabel(UIPanel container)
        {
            // use the same font as the rest of the IPT2 UI
            var font = container.Find<UILabel>("PassengerCount").font;
            
            var panel = container.AddUIComponent<UIPanel>();
            panel.name = "CommuterDestinationInjectedIPT2Container";
            panel.width = container.width - container.autoLayoutPadding.horizontal;
            panel.height = LabelHeight;
            panel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;

            var label = panel.AddUIComponent<UILabel>();
            label.name = "CommuterDestinationInjectedIPT2Label";
            label.font = font;

            // TODO this should be localised
            label.text = "Commuter Destination running";
            label.tooltip = "Commuter Destination has detected that you are using IPT2, so Commuter Destination has been injected into the IPT2 panel.";
            label.tooltipBox.width = 100f;
            label.autoSize = false;
            label.height = LabelHeight;
            label.textScale = 0.875f;
            label.textColor = new Color32(180, 180, 180, byte.MaxValue);
            label.processMarkup = true;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.verticalAlignment = UIVerticalAlignment.Middle;

            var labelX = (panel.width / 2) - (label.width / 2);
            label.position = new Vector3(labelX, 0f);

            label.backgroundSprite = "InfoDisplay";

            return panel;
        }
    }
}

using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSLShowCommuterDestination
{
    public class StopDestinationInfoPanel : UIPanel
    {
        public static StopDestinationInfoPanel instance;

        public ushort stopId;
        public IEnumerable<KeyValuePair<ushort, int>> m_BuildingPopularities;

        private ushort transportLineId;

        private UILabel m_LineNameLabel;
        private UILabel m_StopNameLabel;
        private UILabel m_PassengerCountLabel;

        public override void Start()
        {
            StopDestinationInfoPanel.instance = this;
            base.Start();
            this.SetupPanel();
        }

        public override void Update()
        {
            base.Update();
            if (!this.isVisible)
                return;
            this.CheckForClose();
        }

        public void Show(ushort stopId)
        {
            InstanceID instanceId = InstanceID.Empty;
            instanceId.NetNode = stopId;

            if (!InstanceManager.IsValid(instanceId))
            {
                Debug.LogWarning("Invalid instance ID for StopDestinationInfoPanel");
                this.Hide();
                return;
            }

            this.AttemptToShowIPT2Panel(instanceId);

            this.stopId = stopId;
            var node = Singleton<NetManager>.instance.m_nodes.m_buffer[this.stopId];
            this.transportLineId = node.m_transportLine;

            Debug.Log("Valid instance ID for StopDestinationInfoPanel");
            WorldInfoPanel.HideAllWorldInfoPanels();
            this.m_LineNameLabel.text = Singleton<TransportManager>.instance.GetLineName(this.transportLineId) + " destinations";
            this.m_StopNameLabel.text = "Stop #" + this.GetStopIndex();

            int passengerCount = Singleton<TransportManager>.instance.m_lines.m_buffer[this.transportLineId].CalculatePassengerCount(this.stopId);
            this.m_PassengerCountLabel.text = "Waiting passengers: " + passengerCount;

            this.m_BuildingPopularities = this.GetPositionPopularities();

            ToolsModifierControl.cameraController.SetTarget(instanceId, node.m_position, false);

            this.Show();
            this.LateUpdate();
        }

        public void MoveToPrevStop()
        {
            var prevStop = TransportLine.GetPrevStop(this.stopId);

            this.Show(prevStop);
        }

        public void MoveToNextStop()
        {
            var nextStop = TransportLine.GetNextStop(this.stopId);

            this.Show(nextStop);
        }

        private void AttemptToShowIPT2Panel(InstanceID instanceId)
        {
            Type iptType = Type.GetType("ImprovedPublicTransport2.PublicTransportStopWorldInfoPanel, ImprovedPublicTransport2");

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

            var arguments = new object[] { Singleton<NetManager>.instance.m_nodes.m_buffer[(int)instanceId.NetNode].m_position, instanceId };
            showMethod.Invoke(iptStopPanelInstance, arguments);
        }

        private IEnumerable<KeyValuePair<ushort, int>> GetPositionPopularities()
        {
            ushort nextStop = TransportLine.GetNextStop(this.stopId);
            CitizenManager citizenManager = Singleton<CitizenManager>.instance;
            NetManager netManager = Singleton<NetManager>.instance;
            float num1 = 64f;
            Vector3 stopPosition = netManager.m_nodes.m_buffer[this.stopId].m_position;
            Vector3 nextStopPosition = netManager.m_nodes.m_buffer[nextStop].m_position;
            int num2 = Mathf.Max((int)(((double)stopPosition.x - (double)num1) / 8.0 + 1080.0), 0);
            int num3 = Mathf.Max((int)(((double)stopPosition.z - (double)num1) / 8.0 + 1080.0), 0);
            int num4 = Mathf.Min((int)(((double)stopPosition.x + (double)num1) / 8.0 + 1080.0), 2159);
            int num5 = Mathf.Min((int)(((double)stopPosition.z + (double)num1) / 8.0 + 1080.0), 2159);

            Dictionary<ushort, int> popularities = new Dictionary<ushort, int>();

            for (int index1 = num3; index1 <= num5; ++index1)
            {
                for (int index2 = num2; index2 <= num4; ++index2)
                {
                    ushort citizenInstanceId = citizenManager.m_citizenGrid[index1 * 2160 + index2];
                    while (citizenInstanceId != (ushort)0)
                    {
                        CitizenInstance citizen = citizenManager.m_instances.m_buffer[(int)citizenInstanceId];

                        ushort nextGridInstance = citizen.m_nextGridInstance;

                        if (
                            (citizen.m_flags & CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None
                            && (double)Vector3.SqrMagnitude((Vector3)citizen.m_targetPos - stopPosition) < (double)num1 * (double)num1
                            && citizen.Info.m_citizenAI.TransportArriveAtSource(citizenInstanceId, ref citizen, stopPosition, nextStopPosition)
                        )
                        {
                            if (popularities.ContainsKey(citizen.m_targetBuilding))
                            {
                                int previous = popularities[citizen.m_targetBuilding];

                                popularities.Remove(citizen.m_targetBuilding);

                                popularities.Add(citizen.m_targetBuilding, previous + 1);
                            }
                            else
                            {
                                popularities.Add(citizen.m_targetBuilding, 1);
                            }
                        }

                        citizenInstanceId = nextGridInstance;
                    }
                }
            }

            return popularities.OrderByDescending(key => key.Value);
        }

        private int GetStopIndex()
        {
            ushort stop = Singleton<TransportManager>.instance.m_lines.m_buffer[this.transportLineId].m_stops;
            int index = 1;
            while (stop != 0)
            {
                if (this.stopId == (int)stop)
                {
                    return index;
                }
                ++index;
                stop = TransportLine.GetNextStop(stop);
                if (index >= 32768)
                {
                    break;
                }
            }

            return 0;
        }

        private void CheckForClose()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                this.Hide();
            }
        }

        private void SetupPanel()
        {
            this.name = "StopDestinationInfoPanel";
            this.isVisible = false;
            this.canFocus = true;
            this.isInteractive = true;
            this.anchor = UIAnchorStyle.None;
            this.pivot = UIPivotPoint.MiddleCenter;
            this.transformPosition = new Vector3(1.5f, 0f);
            this.width = 250f;
            this.height = 150f;
            this.backgroundSprite = "MenuPanel";

            this.padding = new RectOffset(10, 10, 5, 5);

            UILabel title = this.AddUIComponent<UILabel>();
            title.name = "TitleLabel";
            title.text = "Commuter Destinations";
            title.relativePosition = new Vector3(10.0f, 10.0f);
            title.textColor = new Color32(231, 220, 161, 255);

            UIPanel container = this.AddUIComponent<UIPanel>();
            container.name = "Container";
            container.width = this.width;
            container.height = 100.0f;
            container.autoLayout = true;
            container.autoLayoutDirection = LayoutDirection.Vertical;
            container.autoLayoutPadding = new RectOffset(0, 0, 5, 5);
            container.autoLayoutStart = LayoutStart.TopLeft;
            container.relativePosition = new Vector3(0.0f, 40.0f);
            container.padding = new RectOffset(10, 10, 5, 5);

            this.m_LineNameLabel = container.AddUIComponent<UILabel>();
            this.m_LineNameLabel.name = "LineNameLabel";
            this.m_LineNameLabel.text = "Line Name";
            this.m_LineNameLabel.textScale = 0.8f;
            this.m_LineNameLabel.relativePosition = new Vector3(0.0f, 20.0f);

            this.m_StopNameLabel = container.AddUIComponent<UILabel>();
            this.m_StopNameLabel.name = "StopNameLabel";
            this.m_StopNameLabel.text = "Stop Name";
            this.m_StopNameLabel.textScale = 0.8f;
            this.m_StopNameLabel.relativePosition = new Vector3(0.0f, 40.0f);

            UIPanel stopNavigation = container.AddUIComponent<UIPanel>();
            stopNavigation.name = "StopNavigation";
            stopNavigation.width = container.width;
            stopNavigation.height = 30.0f;
            stopNavigation.autoLayout = true;
            stopNavigation.autoLayoutDirection = LayoutDirection.Horizontal;
            stopNavigation.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            stopNavigation.autoLayoutStart = LayoutStart.TopLeft;
            stopNavigation.padding = new RectOffset(0, 0, 0, 0);
            stopNavigation.relativePosition = new Vector3(0.0f, 60.0f);

            UIButton previousStop = this.CreateButton(stopNavigation);
            previousStop.name = "PreviousStop";
            previousStop.textPadding = new RectOffset(10, 10, 4, 0);
            previousStop.text = "Previous";
            previousStop.tooltip = "Navigate to the previous stop";
            previousStop.textScale = 0.75f;
            previousStop.size = new Vector2(110f, 30f);
            previousStop.wordWrap = true;
            previousStop.eventClick += new MouseEventHandler(this.OnPrevStopButtonClick);

            UIButton nextStop = this.CreateButton(stopNavigation);
            nextStop.name = "NextStop";
            nextStop.textPadding = new RectOffset(10, 10, 4, 0);
            nextStop.text = "Next";
            nextStop.tooltip = "Navigate to the next stop";
            nextStop.textScale = 0.75f;
            nextStop.size = new Vector2(110f, 30f);
            nextStop.wordWrap = true;
            nextStop.eventClick += new MouseEventHandler(this.OnNextStopButtonClick);

            this.m_PassengerCountLabel = container.AddUIComponent<UILabel>();
            this.m_PassengerCountLabel.name = "PassengerCountLabel";
            this.m_PassengerCountLabel.text = "Passenger Count";
            this.m_PassengerCountLabel.textScale = 0.8f;
            this.m_PassengerCountLabel.relativePosition = new Vector3(0.0f, 90.0f);
        }

        private void OnPrevStopButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.MoveToPrevStop();
        }

        private void OnNextStopButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.MoveToNextStop();
        }

        private UIButton CreateButton(UIComponent parent)
        {
            UIButton uiButton = parent.AddUIComponent<UIButton>();
            uiButton.font = GameObject.Find("(Library) PublicTransportInfoViewPanel").GetComponent<PublicTransportInfoViewPanel>().Find<UILabel>("Label").font;
            uiButton.textPadding = new RectOffset(0, 0, 4, 0);
            uiButton.normalBgSprite = "ButtonMenu";
            uiButton.disabledBgSprite = "ButtonMenuDisabled";
            uiButton.hoveredBgSprite = "ButtonMenuHovered";
            uiButton.focusedBgSprite = "ButtonMenu";
            uiButton.pressedBgSprite = "ButtonMenuPressed";
            uiButton.textColor = new Color32(255, 255, 255, 255);
            uiButton.disabledTextColor = new Color32(7, 7, 7, 255); ;
            uiButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            uiButton.focusedTextColor = new Color32(255, 255, 255, 255);
            uiButton.pressedTextColor = new Color32(30, 30, 44, 255);
            return uiButton;
        }
    }
}

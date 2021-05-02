using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            this.stopId = stopId;
            this.transportLineId = Singleton<NetManager>.instance.m_nodes.m_buffer[this.stopId].m_transportLine;

            Debug.Log("Valid instance ID for StopDestinationInfoPanel");
            WorldInfoPanel.HideAllWorldInfoPanels();
            this.m_StopNameLabel.text = "Stop #" + this.GetStopIndex();
            this.m_LineNameLabel.text = "Line: " + Singleton<TransportManager>.instance.GetLineName(this.transportLineId);

            int passengerCount = Singleton<TransportManager>.instance.m_lines.m_buffer[this.transportLineId].CalculatePassengerCount(this.stopId);
            this.m_PassengerCountLabel.text = "Waiting passengers: " + passengerCount;

            this.m_BuildingPopularities = this.GetPositionPopularities();

            this.Show();
            this.LateUpdate();
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
            int index = 0;
            while (stop != 0)
            {
                if (this.stopId == (int)stop)
                    return index;
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
            this.pivot = UIPivotPoint.BottomRight;
            this.width = 380f;
            this.height = 380f;
            this.backgroundSprite = "InfoBubbleVehicle";

            this.m_LineNameLabel = this.AddUIComponent<UILabel>();
            this.m_LineNameLabel.name = "LineNameLabel";
            this.m_LineNameLabel.text = "Line Name";
            this.m_LineNameLabel.relativePosition = new Vector3(0.0f, 0.0f);

            this.m_StopNameLabel = this.AddUIComponent<UILabel>();
            this.m_StopNameLabel.name = "StopNameLabel";
            this.m_StopNameLabel.text = "Stop Name";
            this.m_StopNameLabel.relativePosition = new Vector3(0.0f, 20.0f);

            this.m_PassengerCountLabel = this.AddUIComponent<UILabel>();
            this.m_PassengerCountLabel.name = "PassengerCountLabel";
            this.m_PassengerCountLabel.text = "Passenger Count";
            this.m_PassengerCountLabel.relativePosition = new Vector3(0.0f, 40.0f);
        }
    }
}

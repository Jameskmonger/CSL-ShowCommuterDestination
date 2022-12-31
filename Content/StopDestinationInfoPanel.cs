using ColossalFramework.UI;
using CSLShowCommuterDestination.Game;
using CSLShowCommuterDestination.Game.Integrations;
using CSLShowCommuterDestination.Graph;
using CSLShowCommuterDestination.UI.Components;
using UnityEngine;

namespace CSLShowCommuterDestination.Content
{
    public class StopDestinationInfoPanel : UIPanel
    {
        static class PanelConfig
        {
            public static float PanelWidth = 300.0f;
            public static float PanelHeight = 150.0f;

            public static float TitleWidth = 250.0f;
            public static float TitleHeight = 36.0f;

            public static float CloseButtonSize = 32.0f;
            public static float CloseButtonY = 3.0f;
            public static float CloseButtonX = PanelWidth - CloseButtonSize - CloseButtonY;
        }

        public static StopDestinationInfoPanel instance;

        public ushort stopId;

        /**
         * The graph of destinations for the currently selected stop.
         * 
         * TODO should this live on the panel?
         */
        public DestinationGraph DestinationGraph { get; private set; }

        private ushort transportLineId;

        private UILabel m_LineNameLabel;
        private UILabel m_StopNameLabel;
        private UILabel m_PassengerCountLabel;

        public StopDestinationInfoPanel()
        {
            name = "StopDestinationInfoPanel";
            canFocus = true;
            isInteractive = true;
            width = PanelConfig.PanelWidth;
            height = PanelConfig.PanelHeight;
            backgroundSprite = "MenuPanel";
            padding = new RectOffset(10, 10, 5, 5);
        }

        public override void Start()
        {
            StopDestinationInfoPanel.instance = this;
            base.Start();
            this.SetupPanel();

            ModIntegrations.CheckEnabledMods();
        }

        public override void Update()
        {
            base.Update();

            this.CheckForClose();
        }

        public void Show(ushort stopId)
        {
            if (ModIntegrations.IsIPT2Enabled())
            {
                IPT2Integration.ShowStopPanel(stopId);
            }

            this.stopId = stopId;
            this.transportLineId = Bridge.GetStopTransportLineId(this.stopId);

            this.DestinationGraph = DestinationGraphGenerator.GenerateGraph(this.stopId);
            
            WorldInfoPanel.HideAllWorldInfoPanels();
            this.m_LineNameLabel.text = Bridge.GetStopLineName(this.stopId) + " destinations";
            this.m_StopNameLabel.text = "Stop #" + Bridge.GetStopIndex(this.stopId);
            this.m_PassengerCountLabel.text = "Waiting passengers: " + Bridge.GetStopPassengerCount(this.stopId);
      
            Bridge.SetCameraOnStop(this.stopId);

            // TODO improve this, its not very reliable
            relativePosition = new Vector3(400f, 400f);

            this.Show();
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
        
        private void CheckForClose()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                this.Hide();
            }
        }

        private void SetupPanel()
        {
            this.isVisible = false;
            this.anchor = UIAnchorStyle.None;
            this.pivot = UIPivotPoint.MiddleCenter;
            this.relativePosition = Vector3.zero;

            CreateTitleBar(this, "TitleBar", "Commuter Destination");

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

            this.m_LineNameLabel = CreateLabel(container, "LineNameLabel", "Line Name");
            this.m_LineNameLabel.relativePosition = new Vector3(0.0f, 20.0f);

            this.m_StopNameLabel = CreateLabel(container, "StopNameLabel", "Stop Name");
            this.m_StopNameLabel.relativePosition = new Vector3(0.0f, 40.0f);

            var stopNavigation = CreateStopNavigation(container);
            stopNavigation.relativePosition = new Vector3(0.0f, 60.0f);

            this.m_PassengerCountLabel = CreateLabel(container, "PassengerCountLabel", "Passenger Count");
            this.m_PassengerCountLabel.relativePosition = new Vector3(0.0f, 90.0f);
        }

        private void OnCloseButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.Hide();
        }

        private void OnPrevStopButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.MoveToPrevStop();
        }

        private void OnNextStopButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.MoveToNextStop();
        }

        private UILabel CreateLabel(UIComponent container, string name, string text)
        {
            var label = container.AddUIComponent<UILabel>();

            label.name = name;
            label.text = text;
            label.textScale = 0.8f;

            return label;
        }

        private UIPanel CreateStopNavigation(UIComponent container)
        {
            UIPanel stopNavigation = container.AddUIComponent<UIPanel>();
            stopNavigation.name = "StopNavigation";
            stopNavigation.width = container.width;
            stopNavigation.height = 30.0f;
            stopNavigation.autoLayout = true;
            stopNavigation.autoLayoutDirection = LayoutDirection.Horizontal;
            stopNavigation.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            stopNavigation.autoLayoutStart = LayoutStart.TopLeft;
            stopNavigation.padding = new RectOffset(0, 0, 0, 0);

            UIButton previousStop = stopNavigation.AddUIComponent<StopPanelNavigationButton>();
            previousStop.name = "PreviousStop";
            previousStop.text = "Previous";
            previousStop.tooltip = "Navigate to the previous stop";
            previousStop.eventClick += new MouseEventHandler(OnPrevStopButtonClick);

            UIButton nextStop = stopNavigation.AddUIComponent<StopPanelNavigationButton>();
            nextStop.name = "NextStop";
            nextStop.text = "Next";
            nextStop.tooltip = "Navigate to the next stop";
            nextStop.eventClick += new MouseEventHandler(OnNextStopButtonClick);

            return stopNavigation;
        }

        private UIPanel CreateTitleBar(UIComponent container, string name, string text)
        {
            UIPanel titleBar = container.AddUIComponent<UIPanel>();
            titleBar.width = PanelConfig.TitleWidth;
            titleBar.height = PanelConfig.TitleHeight;
            titleBar.relativePosition = Vector3.zero;

            UILabel title = titleBar.AddUIComponent<UILabel>();
            title.name = name;
            title.text = text;
            title.isInteractive = false;
            title.width = titleBar.width;
            title.relativePosition = new Vector3(10.0f, 10.0f);
            title.textColor = new Color32(231, 220, 161, 255);

            UIDragHandle dragHandle = titleBar.AddUIComponent<UIDragHandle>();
            dragHandle.width = titleBar.width;
            dragHandle.height = titleBar.height;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = titleBar.parent;

            UIButton closeButton = titleBar.AddUIComponent<UIButton>();
            closeButton.name = "CloseButton";
            closeButton.size = new Vector2(PanelConfig.CloseButtonSize, PanelConfig.CloseButtonSize);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.relativePosition = new Vector3(PanelConfig.CloseButtonX, PanelConfig.CloseButtonY);
            closeButton.eventClick += new MouseEventHandler(OnCloseButtonClick);

            return titleBar;
        }
    }
}

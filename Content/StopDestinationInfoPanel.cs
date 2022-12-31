using ColossalFramework.UI;
using CSLShowCommuterDestination.Game;
using CSLShowCommuterDestination.Game.Integrations;
using CSLShowCommuterDestination.Graph;
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


            UIPanel titleBar = this.AddUIComponent<UIPanel>();
            titleBar.width = PanelConfig.TitleWidth;
            titleBar.height = PanelConfig.TitleHeight;
            titleBar.relativePosition = Vector3.zero;

            UILabel title = titleBar.AddUIComponent<UILabel>();
            title.name = "TitleLabel";
            title.text = "Commuter Destinations";
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
            closeButton.eventClick += new MouseEventHandler(this.OnCloseButtonClick);

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

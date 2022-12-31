using ColossalFramework.UI;
using CSLShowCommuterDestination.Game;
using CSLShowCommuterDestination.Game.Integrations;
using CSLShowCommuterDestination.Graph;
using CSLShowCommuterDestination.UI.Components;
using UnityEngine;

namespace CSLShowCommuterDestination.UI
{
    /// <summary>
    /// The main Commuter Destination info panel, shown when the user opens a
    /// stop.
    /// </summary>
    public class StopDestinationInfoPanel : UIPanel
    {
        /// <summary>
        /// Configuration settings for the panel
        /// </summary>
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

        /// <summary>
        /// Start method from Unity MonoBehaviour.
        /// </summary>
        public override void Start()
        {
            instance = this;
            base.Start();
            SetupPanel();

            // TODO this should not be here - bad separation of concerns
            ModIntegrations.CheckEnabledMods();
        }

        /// <summary>
        /// Update method from Unity MonoBehaviour.
        /// </summary>
        public override void Update()
        {
            base.Update();

            CheckForClose();
        }

        /// <summary>
        /// Show the panel for the given stop.
        /// </summary>
        /// <param name="stopId">the stop ID to show</param>
        public void Show(ushort stopId)
        {
            // TODO this should not be here - bad separation of concerns
            if (ModIntegrations.IsIPT2Enabled())
            {
                IPT2Integration.ShowStopPanel(stopId);
            }

            this.stopId = stopId;
            transportLineId = Bridge.GetStopTransportLineId(this.stopId);

            DestinationGraph = DestinationGraphGenerator.GenerateGraph(this.stopId);
            
            m_LineNameLabel.text = Bridge.GetStopLineName(this.stopId) + " destinations";
            m_StopNameLabel.text = "Stop #" + Bridge.GetStopIndex(this.stopId);
            m_PassengerCountLabel.text = "Waiting passengers: " + Bridge.GetStopPassengerCount(this.stopId);

            Bridge.SetCameraOnStop(this.stopId);

            // TODO improve this, its not very reliable
            relativePosition = new Vector3(400f, 400f);

            Show();
        }

        private void MoveToPrevStop()
        {
            var prevStop = TransportLine.GetPrevStop(stopId);

            Show(prevStop);
        }

        private void MoveToNextStop()
        {
            var nextStop = TransportLine.GetNextStop(stopId);

            Show(nextStop);
        }

        /// <summary>
        /// Close the panel if the Escape key is pressed.
        /// 
        /// Called every frame from Update().
        /// </summary>
        private void CheckForClose()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Hide();
            }
        }

        /// <summary>
        /// Build the UIComponents associated with this panel.
        /// 
        /// This seemingly has to happen in Start() rather than the constructor,
        /// because some of the properties use dynamic setters, so sizing will break if
        /// we try to do it in the constructor.
        /// </summary>
        private void SetupPanel()
        {
            isVisible = false;
            anchor = UIAnchorStyle.None;
            pivot = UIPivotPoint.MiddleCenter;
            relativePosition = Vector3.zero;

            CreateTitleBar(this, "TitleBar", "Commuter Destination");

            UIPanel container = AddUIComponent<UIPanel>();
            container.name = "Container";
            container.width = width;
            container.height = 100.0f;
            container.autoLayout = true;
            container.autoLayoutDirection = LayoutDirection.Vertical;
            container.autoLayoutPadding = new RectOffset(0, 0, 5, 5);
            container.autoLayoutStart = LayoutStart.TopLeft;
            container.relativePosition = new Vector3(0.0f, 40.0f);
            container.padding = new RectOffset(10, 10, 5, 5);

            m_LineNameLabel = CreateLabel(container, "LineNameLabel", "Line Name");
            m_LineNameLabel.relativePosition = new Vector3(0.0f, 20.0f);

            m_StopNameLabel = CreateLabel(container, "StopNameLabel", "Stop Name");
            m_StopNameLabel.relativePosition = new Vector3(0.0f, 40.0f);

            var stopNavigation = CreateStopNavigation(container);
            stopNavigation.relativePosition = new Vector3(0.0f, 60.0f);

            m_PassengerCountLabel = CreateLabel(container, "PassengerCountLabel", "Passenger Count");
            m_PassengerCountLabel.relativePosition = new Vector3(0.0f, 90.0f);
        }

        /// <summary>
        /// Called when the "Close" button is clicked.
        /// </summary>
        private void OnCloseButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Hide();
        }

        /// <summary>
        /// Called when the "Previous Stop" button is clicked.
        /// </summary>
        private void OnPrevStopButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            MoveToPrevStop();
        }

        /// <summary>
        /// Called when the "Next Stop" button is clicked.
        /// </summary>
        private void OnNextStopButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            MoveToNextStop();
        }

        /// <summary>
        /// Create a label with a preset text size.
        /// </summary>
        /// <param name="container">the container to render the label within</param>
        /// <param name="name">the name of the label</param>
        /// <param name="text">the text to display</param>
        /// <returns>the label UIComponent</returns>
        private UILabel CreateLabel(UIComponent container, string name, string text)
        {
            var label = container.AddUIComponent<UILabel>();

            label.name = name;
            label.text = text;
            label.textScale = 0.8f;

            return label;
        }

        /// <summary>
        /// Creates the "stop navigation" components. This contains the "Previous Stop" and "Next Stop" buttons.
        /// </summary>
        /// <param name="container">the container to render the navigation within</param>
        /// <returns>the navigation UIComponent</returns>
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

        /// <summary>
        /// Creates the components for the title bar.
        /// 
        /// <list type="bullet">
        ///     <item>the containing panel</item>
        ///     <item>the label</item>
        ///     <item>a drag handle for the `container` param</item>
        ///     <item>a close button</item>
        /// </list>
        /// </summary>
        /// <param name="container">the container to render the title bar within</param>
        /// <param name="name">the name of the title bar</param>
        /// <param name="text">the title to display</param>
        /// <returns>the title bar UIComponent</returns>
        private UIPanel CreateTitleBar(UIComponent container, string name, string text)
        {
            UIPanel titleBar = container.AddUIComponent<UIPanel>();
            titleBar.name = name;
            titleBar.width = PanelConfig.TitleWidth;
            titleBar.height = PanelConfig.TitleHeight;
            titleBar.relativePosition = Vector3.zero;

            UILabel title = titleBar.AddUIComponent<UILabel>();
            title.name = name + "Title";
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

            // this calls the OnCloseButtonClick method on this StopDestinationInfoPanel instance
            closeButton.eventClick += new MouseEventHandler(OnCloseButtonClick);

            return titleBar;
        }
    }
}

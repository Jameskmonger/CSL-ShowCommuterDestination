using ColossalFramework.UI;
using UnityEngine;

namespace CSLShowCommuterDestination.UI.Components
{
    /// <summary>
    /// This is a button used in the info panel as the "Previous Stop" and "Next Stop" buttons
    /// </summary>
    public class StopPanelNavigationButton : UIButton
    {
        public StopPanelNavigationButton()
        {
            // TODO improve this lookup
            font = GameObject.Find("(Library) PublicTransportInfoViewPanel").GetComponent<PublicTransportInfoViewPanel>().Find<UILabel>("Label").font;

            // TODO this sizing probably won't be suitable for all buttons
            size = new Vector2(110f, 30f);
            textPadding = new RectOffset(10, 10, 4, 0);
            textScale = 0.75f;

            normalBgSprite = "ButtonMenu";
            disabledBgSprite = "ButtonMenuDisabled";
            hoveredBgSprite = "ButtonMenuHovered";
            focusedBgSprite = "ButtonMenu";
            pressedBgSprite = "ButtonMenuPressed";

            textColor = new Color32(255, 255, 255, 255);
            disabledTextColor = new Color32(7, 7, 7, 255); ;
            hoveredTextColor = new Color32(255, 255, 255, 255);
            focusedTextColor = new Color32(255, 255, 255, 255);
            pressedTextColor = new Color32(30, 30, 44, 255);

            wordWrap = true;
        }
    }
}

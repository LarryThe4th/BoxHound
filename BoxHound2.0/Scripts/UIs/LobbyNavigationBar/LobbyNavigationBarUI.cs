using UnityEngine;
using System.Collections;
using System;

namespace BoxHound.UI {
    public class LobbyNavigationBarUI : UIBase
    {

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.LobbyNavigationBarUI,
                UIframework.UIManager.DisplayUIMode.NeverHide,
                UIframework.UIManager.UITypes.UnderBlurEffect,
                false, true, false);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            throw new NotImplementedException();
        }
    }
}


using UnityEngine;
using BoxHound.UIframework;
using System;

namespace BoxHound {
    public class MainMenuManager : SceneManagerBase
    {
        public override void InitScene()
        {
            UIManager.Instance.LoadUI(UIManager.SceneUIs.MainMenuUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.GameMenuUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.HelperWindowUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.SettingsWindowUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.ExitGameDialogUI);
        }
    }
}

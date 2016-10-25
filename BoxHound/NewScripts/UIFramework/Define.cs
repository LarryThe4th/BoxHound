using UnityEngine;
using System.Collections.Generic;

namespace BoxHound.UIFrameWork
{
    public class Define
    {
        #region Properties 
        public enum UICategory
        {
            None = 0,
            MainMenuUI,
            LoadingSceneUI,
            ExitGameUI,
            ExitGameDialogUI,
            HelperDialogUI,
            LobbyNavigationBarUI,
            LobbyUI
        }

        public enum CanvasType {
            AffectByBlur = 0,// Baisc UI on the scene, will effect by blur effect.
            AffectByBlurTop, // Same as the baisc UI but will always on the top of rest of the UI
            NonAffectByBlur  // UIs that won't effect by the blur effect, such as most of the popup window.
        }

        /// <summary>
        /// There are different behaviours when show a UI.
        /// </summary>
        public enum RenderMode {
            DoNothing = 0,  // Do nothing, just show a UI on top of everyone. 
                            // Such as popup window or item description etc.

            HideOther,      // Others will hide when this UI shows up, what a boss. 
                            // Such as the root UI like main menu UI or Lobby UI, these kinds of 
                            // UIs will only have one under the same canvas, once displayed, others
                            // has to hide. (BUT NOT INCLUDE THE HEADER UI)

            NoReturn,       // No need to return to the privous UI. 
                            // Such as Health bar and weapon state UI, these type of UI won't change any thing since display.
        }

        private const string m_UIPerfabFolderPath = "UIPrefab/";
        public string GetUIPerfabFolderPath {
            get { return m_UIPerfabFolderPath; }
        }

        private static Dictionary<UICategory, string> UIPrefabPathList = new Dictionary<UICategory, string>()
        {
            { UICategory.MainMenuUI, m_UIPerfabFolderPath + UICategory.MainMenuUI.ToString() },
            { UICategory.LoadingSceneUI, m_UIPerfabFolderPath + UICategory.LoadingSceneUI.ToString() },
            { UICategory.ExitGameDialogUI, m_UIPerfabFolderPath + UICategory.ExitGameDialogUI.ToString() },
            { UICategory.LobbyNavigationBarUI, m_UIPerfabFolderPath + UICategory.LobbyNavigationBarUI.ToString() },
            { UICategory.LobbyUI, m_UIPerfabFolderPath + UICategory.LobbyUI.ToString() }

        };

        public static string GetUIPerfabPath(UICategory ui) {
            if (UIPrefabPathList.ContainsKey(ui)) return UIPrefabPathList[ui];
            return "";
        }
        #endregion
    }
}

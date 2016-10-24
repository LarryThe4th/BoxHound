using UnityEngine;
using BoxHound.UIframework;

namespace BoxHound
{
    public class LobbyManager : SceneManagerBase
    {
        #region Public variable
        // ---------- Public variable -----------
        public enum LobbyPageCategory
        {
            None = 0,
            RoomBrowser,
            CreateRoom,
            LoadOut,
        }
        #endregion

        public override void InitScene()
        {
            // Under blur
            UIManager.Instance.LoadUI(UIManager.SceneUIs.RoomBrowserUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.CreateRoomUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.LobbyNavigationBarUI);
            // Above blur
            UIManager.Instance.LoadUI(UIManager.SceneUIs.GameMenuUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.HelperWindowUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.SettingsWindowUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.ExitGameDialogUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.LoadingScreenUI);


            // Set room browser as the default page.
            MessageBroadCastManager.OnNavigationButtonClicked(LobbyPageCategory.RoomBrowser);

        }

        protected override void EventRegister(bool reigist)
        {
            // Empty
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    public class RoomMenu : RoomUIBase
    {
        #region Public variables
        // -------------- Public variable -------------
        public Button ResumeGameButton;
        public Button SettingButton;
        public Button ExitRoomButton;
        #endregion

        #region Delegate and event
        public delegate void RoomMenuHandler(bool openMenu);
        public static RoomMenuHandler OnOpenRoomMenu;

        public static void OpenRoomMenu(bool openMenu) {
            if (OnOpenRoomMenu != null) OnOpenRoomMenu(openMenu);
        }

        private void OnEnable()
        {
            OnOpenRoomMenu += DisplayUI;
        }

        private void OnDisable()
        {
            OnOpenRoomMenu -= DisplayUI;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Leave room method.
        /// </summary>
        public void LeaveRoom()
        {
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuDeny);
            ExitRoomButton.interactable = false;
            NetworkManager.Instance.LeaveRoom(); 
        }

        /// <summary>
        /// Close the game menu and return to the game.
        /// </summary>
        public void ResumeGame() {
            OnOpenRoomMenu(false);
        }
        #endregion

        #region Private methods
        // Use this for initialization
        private void Start()
        {
            if (ExitRoomButton)
                ExitRoomButton.onClick.AddListener(delegate { LeaveRoom(); });
            if (ResumeGameButton)
                ResumeGameButton.onClick.AddListener(delegate { ResumeGame(); });

            DisplayUI(false);
        }

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            return GameRoomUI.RoomUITypes.GameMenu;
        }
        #endregion
    }
}
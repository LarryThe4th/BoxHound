using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class RoomMenu : MonoBehaviour
    {
        #region Public variables
        // -------------- Public variable -------------
        public Button ExitRoomButton;
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        private bool m_ShowingMenu = false;
        
        #endregion

        #region Public methods
        public void ShowMenu() {
            m_ShowingMenu = !m_ShowingMenu;
            this.gameObject.SetActive(m_ShowingMenu);
        }

        /// <summary>
        /// Leave room method.
        /// </summary>
        public void LeaveRoom()
        {
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuDeny);
            ExitRoomButton.interactable = false;
            NetWorkManager.Instance.LeaveRoom();
        }
        #endregion

        #region Private methods
        // Use this for initialization
        private void Start()
        {
            if (ExitRoomButton)
                ExitRoomButton.onClick.AddListener(delegate { LeaveRoom(); });
            this.gameObject.SetActive(false);
        }
        #endregion
    }
}
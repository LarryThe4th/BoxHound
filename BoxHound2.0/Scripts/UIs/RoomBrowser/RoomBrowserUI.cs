using UnityEngine;
using System.Collections;
using System;

namespace BoxHound.UI {
    [RequireComponent(typeof(Animator))]
    public class RoomBrowserUI : LobbyPageUIbase
    {
        #region Private Variables
        // -------------- Private variable ------------
        private RoomList m_RoomList;
        private RoomPreview m_RoomPreview;
        private PupupWindowUI m_NoRoomAvailablePopupWindow;

        private Animator m_Animator;
        private readonly string m_AnimationParameterKeyword = "ShowPage";

        private int m_LastUpdateRoomCount = 0;
        #endregion

        /// <summary>
        /// Return current page type.
        /// </summary>
        /// <returns>Current page type.</returns> 
        public override LobbyManager.LobbyPageCategory GetPage()
        {
            return LobbyManager.LobbyPageCategory.RoomBrowser;
        }

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.RoomBrowserUI,
                UIframework.UIManager.DisplayUIMode.Normal,
                UIframework.UIManager.UITypes.UnderBlurEffect,
                false, true);

            // Keep track the room count everytime the room list update.
            m_LastUpdateRoomCount = 0;

            m_RoomList = GetComponentInChildren<RoomList>();
            m_RoomPreview = GetComponentInChildren<RoomPreview>();
            m_NoRoomAvailablePopupWindow = GetComponentInChildren<PupupWindowUI>();
            m_Animator = GetComponent<Animator>();


#if UNITY_EDITOR
            if (m_RoomList == null) Debug.LogError("Couldn't find room list compoment under " + this.gameObject.name);
            if (m_RoomPreview == null) Debug.LogError("Couldn't find room preview compoment under " + this.gameObject.name);
            if (m_NoRoomAvailablePopupWindow == null) Debug.LogError("Couldn't find PopupWindow UI compoment under " + this.gameObject.name);
#endif
            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        /// <summary>
        /// If currently has any available room, retuen TRUE.
        /// </summary>
        public static bool HasAvailableRoom
        {
            get { return PhotonNetwork.GetRoomList().Length != 0 ? true : false; }
        }

        public override void ShowUI()
        {
            base.ShowUI();
            m_Animator.SetBool(m_AnimationParameterKeyword, true);
        }

        public override void HideUI()
        {
            base.HideUI();
            m_Animator.SetBool(m_AnimationParameterKeyword, false);
        }

        public override void EventRegister(bool reigist)
        {
            base.EventRegister(reigist);

            if (reigist)
            {
                // Subscribed to the event when this script is enabled.
                MessageBroadCastManager.RoomListUpdateEvent += UpdateRoomListAndPerview;
            }
            else
            {
                // Unsubscribed to the event when this script is disabled.
                MessageBroadCastManager.RoomListUpdateEvent -= UpdateRoomListAndPerview;
            }
        }

        /// <summary>
        /// Receive boradcast message form the network manager when the room list updated.
        /// </summary>
        private void UpdateRoomListAndPerview()
        {
            

            // No need to update the room list when it is not displaying or not connected to the server.
            if (!IsDisplaying || !NetworkManager.IsConnectedToServer) return;

            if (!m_RoomList || !m_RoomPreview || !m_NoRoomAvailablePopupWindow)
            {
                m_RoomList = GetComponentInChildren<RoomList>();
                m_RoomPreview = GetComponentInChildren<RoomPreview>();
                m_NoRoomAvailablePopupWindow = GetComponentInChildren<PupupWindowUI>();
            }

            // Get the room info form the server, 
            // and populate the room detail element in the room browser's room list.
            m_RoomList.UpdateList();

            // If room count at previous update is ZERO but now there has new room.
            if (m_LastUpdateRoomCount == 0 && HasAvailableRoom)
            {
                // Show room preview.
                m_RoomPreview.ShowRoomPreview(true);
            }
            // If room count at previous update is non-zero but now there has no room available.
            else if (m_LastUpdateRoomCount > 0 && !HasAvailableRoom)
            {
                // Hide the room preview.
                m_RoomPreview.ShowRoomPreview(false);
            }

            // If we have available room at this moment...
            if (HasAvailableRoom)
            {
                m_NoRoomAvailablePopupWindow.PullBack();

                // First check if the room still exist after the update.
                if (string.IsNullOrEmpty(m_RoomPreview.SelectedRoomName) || !CheckSelectedRoomStillExist())
                {
                    // Manually set the first room element in the list as the default option.
                    m_RoomList.GetRoomDetails(0).OnClickRoomListElementButton();
                }
            }
            else
            {
                m_NoRoomAvailablePopupWindow.Popup();
            }

            // Keep track the room count.
            m_LastUpdateRoomCount = PhotonNetwork.GetRoomList().Length;
        }

        /// <summary>
        /// Check if the user selected room is still exist after the room list update.
        /// </summary>
        /// <returns>Return TRUE if the room still exist.</returns>
        private bool CheckSelectedRoomStillExist()
        {
            RoomInfo[] list = PhotonNetwork.GetRoomList();
            for (int index = 0; index < list.Length; index++)
            {
                // If a room's name is same as the user selected room's name form the last update.
                if (string.Compare(list[index].name, m_RoomPreview.SelectedRoomName) == 0)
                {
                    // There is chance that the original room was "deleted",
                    // and someone creates a new room with a same name as the old one.
                    // We need to comperer the room create data to verify it.
                    if (list[index].customProperties.ContainsKey(RoomProperties.RoomCreateDate))
                    {
                        if ((double)list[index].customProperties[RoomProperties.RoomCreateDate] == m_RoomPreview.RoomCreateDate)
                        {
                            // If exist, stay foucs on that room
                            m_RoomList.GetRoomDetails(index).OnClickRoomListElementButton();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void PageDisplayContorl(LobbyManager.LobbyPageCategory type)
        {
            base.PageDisplayContorl(type);

            if (GetPage() == type)
            {
                if (!m_RoomList || !m_RoomPreview)
                {
                    m_RoomList = GetComponentInChildren<RoomList>();
                    m_RoomPreview = GetComponentInChildren<RoomPreview>();
                }

                // Update the room list when display this page
                UpdateRoomListAndPerview();
            } 
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // Empty
        }

    }
}


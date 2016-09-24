using UnityEngine;
using System.Collections;
using System;

namespace Larry.BoxHound
{
    public class RoomBrowser : LobbyPage
    {
        #region Private Variables
        // -------------- Private variable ------------
        private RoomList m_RoomList;
        private RoomPreview m_RoomPreview;
        #endregion

        private void Start() {
            m_RoomList = GetComponentInChildren<RoomList>();
            if (m_RoomList == null) {
                Debug.Log("Couldn't find room list compoment under " + this.gameObject.name + ", please check again.");
            }

            m_RoomPreview = GetComponentInChildren<RoomPreview>();
            if (m_RoomPreview == null) {
                Debug.Log("Couldn't find room preview compoment under " + this.gameObject.name + ", please check again.");
            }

            // Init
            OnReceivedRoomListUpdate();
        }

        private bool HasAvailableRoom()
        {
            return PhotonNetwork.GetRoomList().Length != 0;
        }

        public override void OnReceivedRoomListUpdate()
        {
            base.OnReceivedRoomListUpdate();

            // Safety check
            if (!NetWorkManager.IsConnected) return;
            if (!m_RoomList || !m_RoomPreview) return;

            // Get the room info form the server, 
            // and populate the room detail element in the room browser's room list.
            m_RoomList.PopulateRoomDetail();

            m_RoomPreview.Display(HasAvailableRoom());

            // If we have available room at this moment...
            if (HasAvailableRoom())
            {
                // First check if the room still exist after the update.
                if (CheckIfRoomStillExist()) return;

                // If the room is no longer exist, also the previously selected room name is empty,
                // that means the user didn't select any room yet,
                // and mosty its because the room browser just initialized.
                if (string.IsNullOrEmpty(m_RoomPreview.SelectedRoomName))
                {
                    // Set the first room element in the list as the default option.
                    m_RoomPreview.OnRoomSelected(null, new RoomDetailEventArgs() { Index = 0, RoomInfo = PhotonNetwork.GetRoomList()[0] });
                }
            }
        }

        /// <summary>
        /// Check if the user selected room is still exist after the room list update.
        /// </summary>
        /// <returns>Return TRUE if the room still exist.</returns>
        private bool CheckIfRoomStillExist() {
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
                            m_RoomPreview.OnRoomSelected(
                                null, new RoomDetailEventArgs()
                                { Index = index, RoomInfo = list[index] });
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void ShowPage(bool show)
        {
            base.ShowPage(show);
            if (show) 
                OnReceivedRoomListUpdate();
        }

        public override LobbyManager.Pages GetLobbyPage()
        {
            return LobbyManager.Pages.RoomBrowser;
        }
    }
}

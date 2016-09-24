using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Larry.BoxHound
{
    public class RoomDetailEventArgs : EventArgs {
        public int Index { get; set; }
        public RoomInfo RoomInfo { get; set; }
    }

    public class RoomDetails : MonoBehaviour
    {
        #region public Variables
        // -------------- Public variable -------------
        [Tooltip("The cache of text UI compoment.")]
        public Text RoomDetailText;             // The cache of text UI compoment.

        public RoomInfo GetRoomInfo {           // The getter for the room information.
            get { return m_RoomInfo; } }

        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private int m_IndexInRoomList = -1;     // The index of this element in the list.

        private RoomInfo m_RoomInfo = null;     // All the information about this room stores in here.
        #endregion

        #region Room selected event related.
        public delegate void RoomSelectedEventHandler(RoomDetails soruce, RoomDetailEventArgs args);
        public static event RoomSelectedEventHandler RoomSelected;

        protected virtual void OnRoomSelected() {
            if (RoomSelected != null) RoomSelected(this, 
                new RoomDetailEventArgs() { Index = m_IndexInRoomList, RoomInfo = m_RoomInfo });
        }
        #endregion

        /// <summary>
        /// Called when instantiate by the lobby manager.
        /// </summary>
        public void Init(int index, RoomInfo roomInfo) {
            m_IndexInRoomList = index;
            m_RoomInfo = roomInfo;
            RoomDetailText.text = m_RoomInfo.name;
        }

        public void EnableDisplay(bool enable) {
            this.gameObject.SetActive(enable);
        }

        public void OnSeletedRoomDetail()
        {
            // Send the event to all the subscribes.
            OnRoomSelected();
        }
    }
}

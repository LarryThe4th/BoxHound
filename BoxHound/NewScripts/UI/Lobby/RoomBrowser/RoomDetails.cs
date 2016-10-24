using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace BoxHound.Larry
{
    public class RoomDetails : MonoBehaviour
    {
        #region Delegate and Event
        public delegate void RoomSelectedEventHandler(RoomDetails source, EventArguments args);
        public static event RoomSelectedEventHandler RoomSelected;

        public class EventArguments : EventArgs
        {
            public int Index { get; set; }
            public RoomInfo RoomInfo { get; set; }
        }

        protected virtual void OnRoomSelected()
        {
            // Send message to all the subscriblers.
            if (RoomSelected != null) RoomSelected(this,
                new EventArguments() { Index = m_IndexInRoomList, RoomInfo = m_RoomInfo });
        }

        private void OnEnable()
        {
            // Subscrible to the room details class. 
            // When user selected a room it should update the preview UI.
            RoomSelected += ChangeButtonColor;
        }

        private void OnDisable()
        {
            RoomSelected -= ChangeButtonColor;
        }
        #endregion

        #region public Variables
        // -------------- Public variable -------------
        [Tooltip("The cache of Room name text UI compoment.")]
        public Text RoomNameText;             // The cache of text UI compoment.

        [Tooltip("The cache of Room index text UI compoment.")]
        public Text RoomIndexText;             // The cache of text UI compoment.

        [Tooltip("The cache of Image UI compoment.")]
        public Image DecorationBar;

        public Color SelectedDecorationBarColor;
        public Color DeselectedDecorationBarColor;

        public RoomInfo GetRoomInfo {           // The getter for the room information.
            get { return m_RoomInfo; } }

        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private Button m_RoomDetailButton;

        private int m_IndexInRoomList = -1;     // The index of this element in the list.

        private RoomInfo m_RoomInfo = null;     // All the information about this room stores in here.
        #endregion

        /// <summary>
        /// Called when instantiate by the lobby manager.
        /// </summary>
        public void Init(int index, RoomInfo roomInfo) {
            m_IndexInRoomList = -1;
            m_RoomInfo = null;

            if (!m_RoomDetailButton) {
                m_RoomDetailButton = GetComponent<Button>();
                m_RoomDetailButton.onClick.AddListener(delegate { OnSeletedRoomDetail(); });
            }
            m_IndexInRoomList = index;
            m_RoomInfo = roomInfo;
            RoomNameText.text = m_RoomInfo.name;
            RoomIndexText.text = (m_IndexInRoomList + 1).ToString();
        }

        public void EnableDisplay(bool enable) {
            this.gameObject.SetActive(enable);
        }

        public void OnSeletedRoomDetail()
        {
            // Send the event to all the subscribes.
            OnRoomSelected();
        }

        public void ChangeButtonColor(RoomDetails source, EventArguments args) {
            if (!m_RoomDetailButton)
            {
                m_RoomDetailButton = GetComponent<Button>();
                m_RoomDetailButton.onClick.AddListener(delegate { OnSeletedRoomDetail(); });
            }

            if (source == this)
            {
                DecorationBar.color = SelectedDecorationBarColor;
                // Set the selected button non-interactable so it will change its color, 
                // also it can prevent user form accidentally clicked on the button multiple times.
                m_RoomDetailButton.interactable = false;
            }
            else {
                DecorationBar.color = DeselectedDecorationBarColor;
                // Reset to interactable.
                m_RoomDetailButton.interactable = true;
            }
        }
    }
}

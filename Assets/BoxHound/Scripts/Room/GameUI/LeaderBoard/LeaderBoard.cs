using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class LeaderBoard : Photon.PunBehaviour
    {
        #region Public Variables
        // -------------- Public variable -------------
        public Text Header;
        public Text RoomInfoText;

        public SingleRowPlayerList m_SingleRow;
        public DoubleRowPlayerList m_DoubleRow;
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private string m_GameMap = "";
        private string m_GameMode = "";
        private int m_TimeLimit = 0;
        private int m_TicketLimit = 0;
        #endregion

        /// <summary>
        /// Use this for initialization
        /// </summary>
        public void Init(bool useDoubleList) {
            if (!NetWorkManager.IsConnected) return;

            // Initialize the single row player list and switch on and off based on current gamemode.
            m_SingleRow.Use(!useDoubleList);

            // Initialize the double row player list and switch on and off based on current gamemode.
            m_DoubleRow.Use(useDoubleList);

            // Get current room information form the server.
            Room currentRoom = PhotonNetwork.room;
            if (RoomProperties.CheckIfRoomContantsKey(currentRoom, RoomProperties.MapIndex)) {
                m_GameMap = GameMapManager.GetGameMap((int)currentRoom.customProperties[RoomProperties.MapIndex]).GameMapName;
            }

            if (RoomProperties.CheckIfRoomContantsKey(currentRoom, RoomProperties.GameModeIndex)) {
                m_GameMode = GameModeManager.GetGameModeDetail(
                    (int)currentRoom.customProperties[RoomProperties.GameModeIndex]).GameModeName.ToUpper();
            }
                
            if (RoomProperties.CheckIfRoomContantsKey(currentRoom, RoomProperties.TimeLimit))
                m_TimeLimit = (int)currentRoom.customProperties[RoomProperties.TimeLimit];

            if (RoomProperties.CheckIfRoomContantsKey(currentRoom, RoomProperties.ScoreLimit))
                m_TicketLimit = (int)currentRoom.customProperties[RoomProperties.ScoreLimit];

            // Initialize the leader board header and the room info.
            UpdateRoomInfoText();

            ShowLeaderBoard(false);
        }

        // private float m_Timer = 0.0f;
        public void UpdateRoomInfoText() {
            Header.text = PhotonNetwork.room.name + " - " + m_GameMap;
            RoomInfoText.text = m_GameMode + ": " + m_TicketLimit + " TICKETS " + m_TimeLimit + " MIN - TIME LEFT: 00:00";
            //m_Timer += Time.deltaTime;

            //var minutes = m_Timer / 60; //Divide the guiTime by sixty to get the minutes.
            //var seconds = m_Timer % 60;//Use the euclidean division for the seconds.

            ////update the label value
            //RoomInfoText.text = "Timer: " + minutes + ":" + seconds;
        }

        public void ShowLeaderBoard(bool show) {
            this.gameObject.SetActive(show);
        }
    }
}

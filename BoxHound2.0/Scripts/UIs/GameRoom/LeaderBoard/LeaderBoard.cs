using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    public class LeaderBoard : RoomUIBase
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
        // private int m_TicketLimit = 0;

        private bool m_useDoubleList = false;
        #endregion

        /// <summary>
        /// Use this for initialization
        /// </summary>
        public void Init(bool useDoubleList) {
            if (!NetworkManager.IsConnectedToServer) return;

            m_useDoubleList = useDoubleList;

            // Initialize the single row player list and switch on and off based on current gamemode.
            m_SingleRow.Use(!m_useDoubleList);

            // Initialize the double row player list and switch on and off based on current gamemode.
            m_DoubleRow.Use(m_useDoubleList);


            // Get current room information form the server.
            Room currentRoom = PhotonNetwork.room;
            if (RoomProperties.ContantsKey(currentRoom, RoomProperties.MapIndex)) {
                m_GameMap = GameMapManager.GetGameMap((int)currentRoom.customProperties[RoomProperties.MapIndex]).GameMapName;
            }

            if (RoomProperties.ContantsKey(currentRoom, RoomProperties.GameModeIndex)) {
                m_GameMode = GameModeManager.GetGameModeDetail(
                    (int)currentRoom.customProperties[RoomProperties.GameModeIndex]).GameModeName.ToUpper();
            }


            if (RoomProperties.ContantsKey(currentRoom, RoomProperties.RoundTimeLimit))
                m_TimeLimit = (int)currentRoom.customProperties[RoomProperties.RoundTimeLimit];


            // Initialize the leader board header and the room info.
            UpdateRoomInfoText();

            DisplayUI(false);
        }

        public override void DisplayUI(bool show)
        {
            base.DisplayUI(show);
            if (!show) return;

            if (m_useDoubleList)
            {
                // m_DoubleRow.UpdateList();
            }
            else {
                m_SingleRow.UpdateList();
            }

            UpdateRoomInfoText();
        }

        public void UpdateRoomInfoText() {
            if (!m_IsDisplaying) return;

            double timeLeft = RoomManager.CurrentGameMode.GetRoundTimeLeft();
            int minutesLeft = Mathf.FloorToInt((float)timeLeft / 60);
            int secondsLeft = Mathf.FloorToInt((float)timeLeft) % 60;

            if (minutesLeft <= 0 || minutesLeft <= 0)
            {
                Header.text = PhotonNetwork.room.name + " - " + m_GameMap;
                RoomInfoText.text = m_GameMode + ": " + m_TimeLimit + " 分 - 結果発表";
            }
            else
            {
                Header.text = PhotonNetwork.room.name + " - " + m_GameMap;
                RoomInfoText.text = m_GameMode + ": " + m_TimeLimit + " 分 - 残り時間: " +
                    minutesLeft.ToString() + ":" + secondsLeft.ToString("00");
            }
        }

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            return GameRoomUI.RoomUITypes.LeaderBorad;
        }
    }
}

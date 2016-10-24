using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BoxHound.UI
{
    public class LeaderBoardUI : UIBase
    {
        #region Public Variables
        // -------------- Public variable -------------
        public Text Header;
        public Text RoomInfoText;
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private string m_GameMap = "";
        private string m_GameMode = "";
        private int m_TimeLimit = 0;
        // private int m_TicketLimit = 0;

        [SerializeField]
        private GameObject LeaderBoardsRoot;

        private bool m_useDoubleList = false;
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            // Safty check.
            if (!NetworkManager.IsConnectedToServer) return;

            //Properties = new UIProperties(
            //    UIframework.UIManager.SceneUIs.LeaderBoradUI,
            //    UIframework.UIManager.DisplayUIMode.Normal,
            //    UIframework.UIManager.UITypes.UnderBlurEffect,
            //    true, true);

#if UNITY_EDITOR
            if (!LeaderBoardsRoot) Debug.Log("LeaderBoardsRoot is null");
#endif
            // Get current room information form the server.
            Room currentRoom = PhotonNetwork.room;
            if (RoomProperties.ContantsKey(currentRoom, RoomProperties.MapIndex))
            {
                m_GameMap = GameMapManager.GetGameMap((int)currentRoom.customProperties[RoomProperties.MapIndex]).GameMapName;
            }

            if (RoomProperties.ContantsKey(currentRoom, RoomProperties.GameModeIndex))
            {
                m_GameMode = GameModeManager.GetGameModeDetail(
                    (int)currentRoom.customProperties[RoomProperties.GameModeIndex]).GameModeName.ToUpper();
            }

            if (RoomProperties.ContantsKey(currentRoom, RoomProperties.RoundTimeLimit))
                m_TimeLimit = (int)currentRoom.customProperties[RoomProperties.RoundTimeLimit];

            // Initialize the leader board header and the room info.
            UpdateRoomInfoText();

        }

        public override void ShowUI()
        {
            base.ShowUI();
            LeaderBoardsRoot.SetActive(true);
            UpdateRoomInfoText();
        }

        public override void HideUI()
        {
            base.HideUI();
            LeaderBoardsRoot.SetActive(false);
        }

        public void UpdateRoomInfoText()
        {
            if (!IsDisplaying) return;

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

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            throw new NotImplementedException();
        }
    }
}

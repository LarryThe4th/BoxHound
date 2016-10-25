using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BoxHound.UIframework;

namespace BoxHound.UI
{
    public class LeaderBoardUI : UIBase
    {
        #region Public Variables
        // -------------- Public variable -------------
        public Text Header;
        public Text RoomInfoText;
        public Transform ListRoot;
        public GameObject LeaderBoardListPrefab;
        public GameObject LeaderBoradListElementPrefab;
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private string m_GameMap = "";
        private string m_GameMode = "";
        private int m_TimeLimit = 0;

        private List<LeaderBoradSingleList> m_lists = new List<LeaderBoradSingleList>();
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.LeaderBoardUI,
                UIframework.UIManager.DisplayUIMode.Normal,
                UIframework.UIManager.UITypes.AboveBlurEffect, true, true);

            // Store all the leaderborad list
            m_lists.Clear();
            foreach (var item in ListRoot.GetComponentsInChildren<LeaderBoradSingleList>())
            {
                item.Init();
                m_lists.Add(item);
            }

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
            UpdateHeaederText();
        }


        public override void ShowUI()
        {
            if (!IsDisplaying) {
                m_CanvasGroup.alpha = 1.0f;
                IsDisplaying = true;
            }
        }

        public override void HideUI()
        {
            if (IsDisplaying)
            {
                m_CanvasGroup.alpha = 0.0f;
                IsDisplaying = false;
            }
        }

        public void UpdateHeaederText() {
            if (!IsDisplaying) return;

            double timeLeft = GameRoomManager.CurrentGameMode.GetRoundTimeLeft();
            int minutesLeft = Mathf.FloorToInt((float)timeLeft / 60);
            int secondsLeft = Mathf.FloorToInt((float)timeLeft) % 60;

            if (minutesLeft <= 0 || minutesLeft <= 0)
            {
                Header.text = PhotonNetwork.room.name + " - " + m_GameMap;
                RoomInfoText.text = m_GameMode + ": " + m_TimeLimit + " " + GameLanguageManager.TimeUnit + " - " + GameLanguageManager.RoundEnd + ": ";
            }
            else
            {
                Header.text = PhotonNetwork.room.name + " - " + m_GameMap;
                RoomInfoText.text = m_GameMode + ": " + m_TimeLimit + " " + GameLanguageManager.TimeUnit + " - " + GameLanguageManager.TimeLeft + ": " +
                    minutesLeft.ToString() + ":" + secondsLeft.ToString("00");
            }
        }

        public override void Process()
        {
            base.Process();

            if (Input.GetKey(KeyCode.Tab) && GameRoomManager.CurrentPhase == GameRoomManager.GamePhase.RunningGame)
            {
                ShowUI();
            }
            else {
                HideUI();
            }

            // Update Header info
            UpdateHeaederText();

            // Update all list.
            foreach (var item in m_lists) {
                item.UpdateList();
            }
        }


        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // Enmpty
        }
    }
}

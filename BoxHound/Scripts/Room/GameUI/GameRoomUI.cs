using UnityEngine;
using System.Collections;
using BoxHound.Manager;
using System;

namespace BoxHound.UI
{
    public class GameRoomUI : UIBase
    {
        #region Public variables
        // -------------- Public variable -------------
        public enum RoomUITypes {
            GameMenu,
            GameOpening,
            PlayerHUD,
            LeaderBorad,
            MessageCenter,
        }

        [Header("Game UIs")]
        public LeaderBoardUI leaderBoard;  // The Leader borad UI.
        // public GameOpening GameOpeningUI;  // The Game opening UI.
        public PlayerHUD PlayerHUDUI;      // The player head-up display UI.
        public MessageCenter MessageCenterUI; // The room message center UI.

        public static GameRoomUI Instance;
        #endregion

        #region Private variables
        // -------------- Private variable ------------
        // Temp
        private bool m_AllInited = false;

        private bool m_StopUIProcess = false;
        #endregion

        #region Private methods.
        // Update is called once per frame
        private void Update()
        {
            if (!m_AllInited) return;
            if (m_StopUIProcess) return;
            //// Update game opening UI.
            //GameOpeningUI.Process();
            GameUIControl();
        }

        // FixedUpdate is called once at the end of per frame
        public void FixedUpdate() {
            if (!m_AllInited) return;
            if (m_StopUIProcess) return;
            PlayerHUDUI.ProcessHUD();
        }

        private void GameUIControl()
        {
            if (m_StopUIProcess) return;

            //if (Input.GetKey(KeyCode.Tab) && RoomManager.CurrentPhase == RoomManager.GamePhase.RunningGame)
            //{
            //    leaderBoard.DisplayUI(true);
            //}
            //else
            //{
            //    leaderBoard.DisplayUI(false);
            //}

            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    if (RoomManager.CurrentPhase == RoomManager.GamePhase.RoundStart ||
            //        RoomManager.CurrentPhase == RoomManager.GamePhase.RunningGame) {
            //        // When the room menu is displaying, 
            //        // press Escape key again to resume the game.
            //        if (RoomMenuUI.IsDisplaying())
            //            RoomMenu.OnOpenRoomMenu(false);
            //        else
            //            RoomMenu.OnOpenRoomMenu(true);
            //    }
            //}

            if (Input.GetKeyDown(KeyCode.Return))
            {
                // If current game progress is in the Preparation phase.
                //if (RoomManager.CurrentPhase == RoomManager.GamePhase.Preparation) {
                //    GameOpeningUI.OnClickStart();
                //}
            }
        }
        #endregion

        public override void InitUI()
        {
            base.InitUI();


            m_AllInited = false;
            m_StopUIProcess = false;

            if (!NetworkManager.IsConnectedToServer) return;


            Instance = this;
            Properties = new UIProperties(UIframework.UIManager.SceneUIs.GameRoomUI, UIframework.UIManager.DisplayUIMode.NeverHide, UIframework.UIManager.UITypes.UnderBlurEffect, false, true);

            leaderBoard = GetComponentInChildren<LeaderBoardUI>();
            // leaderBoard.Init(GameRoomManager.CurrentGameMode.IsUsingTeamRule());

            //GameOpeningUI = GetComponentInChildren<GameOpening>();
            //GameOpeningUI.Init(RoomManager.CurrentGameMode.IsUsingTeamRule());


            PlayerHUDUI = GetComponentInChildren<PlayerHUD>();
            PlayerHUDUI.Init();

            MessageCenterUI = GetComponentInChildren<MessageCenter>();
            MessageCenterUI.Init();

            m_AllInited = true;

        }

        public void StopUpdateUI(bool stop) {
            m_StopUIProcess = stop;
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // empty
        }
    }
}

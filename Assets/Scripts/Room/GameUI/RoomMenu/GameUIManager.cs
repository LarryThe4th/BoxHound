using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    public class GameUIManager : MonoBehaviour
    {
        #region Public variables
        // -------------- Public variable -------------
        [Header("Game UIs")]
        public LeaderBoard leaderBoardUI;         // The Leader borad UI.
        public RoomMenu RoomMenuUI;               // The Room menu UI.
        public GameOpening GameOpeningUI;         // The Game opening UI.
        public GameInfo GameInfoUI;               // The player UI.

        #endregion

        #region Private variables
        // -------------- Private variable ------------
        private static GameUIManager m_Instance;     // The singleton design of room UI manager.
        #endregion

        #region Private methods.
        // Use this for initialization
        private void Start()
        {
            m_Instance = this;
            if (!leaderBoardUI) leaderBoardUI = GetComponentInChildren<LeaderBoard>();
            if (!RoomMenuUI) RoomMenuUI = GetComponentInChildren<RoomMenu>();
            if (!GameOpeningUI) GameOpeningUI = GetComponentInChildren<GameOpening>();
            if (!GameInfoUI) GameInfoUI = GetComponentInChildren<GameInfo>();
        }

        public void InitAllUI(bool enableTeam) {
            if (!NetWorkManager.IsConnected) return;
            if (!leaderBoardUI) leaderBoardUI = GetComponentInChildren<LeaderBoard>();
            leaderBoardUI.Init(enableTeam);
            if (!GameOpeningUI) GameOpeningUI = GetComponentInChildren<GameOpening>();
            GameOpeningUI.Init(enableTeam);
            if (!GameInfoUI) GameInfoUI = GetComponentInChildren<GameInfo>();
            GameInfoUI.Init();
        }

        // Update is called once per frame
        private void Update()
        {
            GameUIControl();
        }

        private void GameUIControl()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !GameOpeningUI.gameObject.GetActive())
            {
                leaderBoardUI.ShowLeaderBoard(true);
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                leaderBoardUI.ShowLeaderBoard(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !GameOpeningUI.gameObject.GetActive())
            {
                RoomMenuUI.ShowMenu();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (GameOpeningUI.TeamSelected && GameOpeningUI.gameObject.GetActive())
                {
                    GameOpeningUI.Show(false);
                }
            }
        }
        #endregion

        #region Singleton pattern of the network manager
        public static GameUIManager Instance
        {
            get
            {
                if (m_Instance == null) {
                    m_Instance = FindObjectOfType<GameUIManager>();
                }
                return m_Instance;
            }
        }
        #endregion
    }
}

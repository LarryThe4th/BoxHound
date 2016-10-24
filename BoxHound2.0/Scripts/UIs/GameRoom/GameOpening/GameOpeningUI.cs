using UnityEngine;
using UnityEngine.UI;
using BoxHound.UIframework;

namespace BoxHound.UI {
    [RequireComponent(typeof(Animator))]
    public class GameOpeningUI : UIBase
    {
        #region Private variable
        // ---------------- Private variable ---------------
        [SerializeField]
        private Text m_RoomAndMapName;
        [SerializeField]
        private Text m_RoomDetail;
        [SerializeField]
        private Text m_ObjectiveHeader;
        [SerializeField]
        private Text m_ObjectiveDescription;
        [SerializeField]
        private Button m_StartGameButton;
        [SerializeField]
        private Text m_StartGameButtonText;

        [SerializeField]
        private Animator m_Animator;
        private readonly string m_AnimatorParameterKeyword = "ShowWindow";

        // Text for the start game button.
        private string m_PleaseStandBy = "";
        private string m_StartGame = "";
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.GameOpeningUI, 
                UIframework.UIManager.DisplayUIMode.Normal, 
                UIframework.UIManager.UITypes.AboveBlurEffect, 
                false, true);

#if UNITY_EDITOR
            if (!m_RoomAndMapName) Debug.Log("m_RoomAndMapName is null.");
            if (!m_RoomDetail) Debug.Log("m_RoomDetail is null.");
            if (!m_ObjectiveHeader) Debug.Log("m_ObjectiveHeader is null.");
            if (!m_ObjectiveDescription) Debug.Log("m_ObjectiveDescription is null.");
            if (!m_StartGameButton) Debug.Log("m_StartGameButton is null.");
            if (!m_StartGameButtonText) Debug.Log("m_StartGameButtonText is null.");
#endif
            m_Animator = GetComponent<Animator>();
            m_StartGameButton.onClick.AddListener(delegate { OnClickedStartGameButton(); });

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        private void UpdateRoomInfoDetail() {
            // Get current room information form the server.
            Room currentRoom = PhotonNetwork.room;

            double timeLeft = GameRoomManager.CurrentGameMode.GetRoundTimeLeft();
            int minutesLeft = Mathf.FloorToInt((float)timeLeft / 60);
            int secondsLeft = Mathf.FloorToInt((float)timeLeft) % 60;

            if (minutesLeft <= 0 && secondsLeft <= 0) {
                GameRoomManager.Instance.ProcessGamePhase(GameRoomManager.GamePhase.RoundEnded);
                UIManager.Instance.HideUI(this);
            }

            m_RoomAndMapName.text = PhotonNetwork.room.name + " - " + GameMapManager.GetGameMap((int)(currentRoom.customProperties[RoomProperties.MapIndex])).GameMapName;

            m_RoomDetail.text = GameModeManager.GetGameModeName(GameModeManager.GameModes.FreeForAll) + " - " +
                (int)currentRoom.customProperties[RoomProperties.RoundTimeLimit]  + " " + GameLanguageManager.TimeUnit + " - " +
                GameLanguageManager.TimeLeft + " " + minutesLeft.ToString() + ":" + secondsLeft.ToString("00");
        }

        private void UpdateStartButton() {
            // If preparation phase is ended, 
            // after player picked a team or just press the start button, join game immediate.
            if (GameRoomManager.CurrentGameMode.GetPreparationPhaseTimeLeft() == -1)
            {
                m_StartGameButton.interactable = true;
                m_StartGameButtonText.text = m_StartGame;
            }
            else {
                m_StartGameButton.interactable = false;
                m_StartGameButtonText.text = m_PleaseStandBy + 
                    " (" + Mathf.FloorToInt((float)GameRoomManager.CurrentGameMode.GetPreparationPhaseTimeLeft()) + ")";
            }
        }

        private void OnClickedStartGameButton() {
            // Boradcast this message.
            MessageBroadCastManager.OnPlayerStartGame();
            UIManager.Instance.HideUI(this);
        }

        public override void ShowUI()
        {
            base.ShowUI();
            m_Animator.SetBool(m_AnimatorParameterKeyword, true);
        }

        public override void HideUI()
        {
            base.HideUI();
            m_Animator.SetBool(m_AnimatorParameterKeyword, false);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_PleaseStandBy = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Open_StartGameTextPleaseStandBy, language);
            m_StartGame = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Open_StartGameTextStart, language);
        }

        public override void Process()
        {
            base.Process();

            // Only update room info detail when this UI is displaying.
            if (IsDisplaying) {
                UpdateRoomInfoDetail();
                UpdateStartButton();
            } 
        }
    }
}

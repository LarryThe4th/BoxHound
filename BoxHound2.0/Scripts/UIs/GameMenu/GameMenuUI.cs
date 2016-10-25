using UnityEngine;
using UnityEngine.UI;
using BoxHound.UIframework;
using System;

namespace BoxHound.UI {
    [RequireComponent(typeof(Animator))]
    public class GameMenuUI : UIBase
    {
        public static bool EnbaleHotKey = true;

        #region Private variable
        private Animator m_Animator;

        [SerializeField]
        private Button m_ReturnButton;

        [SerializeField]
        private Button m_SettingsButton;

        [SerializeField]
        private Button m_HelpButton;

        [SerializeField]
        private Button m_AboutButton;

        [SerializeField]
        private Button m_ExitRoomButton;

        [SerializeField]
        private Text m_ExitRoomButtonText;

        private readonly int m_AnimatorParameterKey = Animator.StringToHash("ShowMenu");

        private static bool m_InMenu = false;
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.GameMenuUI,
                UIframework.UIManager.DisplayUIMode.HideOthers,
                UIframework.UIManager.UITypes.AboveBlurEffect,
                false, true, true);

            m_Animator = GetComponent<Animator>();

#if UNITY_EDITOR
            if (!m_ExitRoomButton) Debug.LogError("m_ExitRoomButton is null");
            if (!m_ExitRoomButtonText) Debug.LogError("m_ExitRoomButtonText is null");
            if (!m_AboutButton) Debug.LogError("m_AboutButton is null");
            if (!m_HelpButton) Debug.LogError("m_HelpButton is null");
            if (!m_SettingsButton) Debug.LogError("m_SettingsButton is null");
            if (!m_ReturnButton) Debug.LogError("m_Button is null");
#endif

            m_AboutButton.onClick.AddListener(delegate { OnClickedAbout(); });
            m_HelpButton.onClick.AddListener(delegate { OnClickedHelp(); });
            m_SettingsButton.onClick.AddListener(delegate { OnClickedSettings(); });
            m_ReturnButton.onClick.AddListener(delegate { OnClickedReturn(); });
            m_ExitRoomButton.onClick.AddListener(delegate { OnClickedExitRoom(); });

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public void OnClickedReturn() {
            UIManager.Instance.HideUI(this);
            MessageBroadCastManager.OnGamePause(false);
        }

        public void OnClickedSettings() {
            UIManager.Instance.ShowUI(UIManager.SceneUIs.SettingsWindowUI);
        }

        private void OnClickedHelp() {
            UIManager.Instance.ShowUI(UIManager.SceneUIs.HelperWindowUI);
        }

        private void OnClickedAbout() {
            UIManager.Instance.ShowUI(UIManager.SceneUIs.AboutWindowUI);
        }

        private void OnClickedExitRoom()
        {
            // TODO: need confirmation
            UIframework.UIManager.Instance.ResetUnderBlurCanvasRenderCamera();
            NetworkManager.Instance.LeaveRoom();
        }

        public override void ShowUI()
        {
            base.ShowUI();

            // Exit room button will only display when in room.
            m_ExitRoomButton.gameObject.SetActive(LoadingScreenManager.IsInGame);

            m_Animator.SetBool(m_AnimatorParameterKey, true);
        }

        public override void HideUI()
        {
            base.HideUI();

            m_Animator.SetBool(m_AnimatorParameterKey, false);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_ReturnButton.GetComponentInChildren<Text>().text = 
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Menu_ReturnButtonText, language);
            m_SettingsButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Menu_SettingsButtonText, language);
            m_HelpButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Menu_HelpButtonText, language);
            m_AboutButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Menu_AboutButtonText, language);
            m_ExitRoomButtonText.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Menu_ExitRoomButtonText, language);
        }

        public override void Process()
        {
            base.Process();

            if (Input.GetKeyDown(KeyCode.Escape) && EnbaleHotKey)
            {
                if (!IsDisplaying && !m_InMenu)
                {
                    m_InMenu = true;
                    MessageBroadCastManager.OnGamePause(true);
                    UIManager.Instance.ShowUI(Properties.GetSceneUI);
                }
                else
                {
                    
                    m_InMenu = false;
                    // A ltiile hacky way
                    MessageBroadCastManager.OnGamePause(false);
                    UIManager.Instance.HideUI(Properties.GetSceneUI);
                }
            }
        }
    }

}

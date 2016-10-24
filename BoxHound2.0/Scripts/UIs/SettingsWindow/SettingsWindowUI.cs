using UnityEngine;
using UnityEngine.UI;
using System;
using BoxHound.UIframework;

namespace BoxHound.UI {
    // TODO: I should create a settings manager to handle these.
    [RequireComponent(typeof(Animator))]
    public class SettingsWindowUI : UIBase
    {
        #region private variable
        // --------------- private variable --------------
        private Animator m_Animator;
        private readonly string m_AnimatorParameterKeyword = "ShowWindow";

        [SerializeField]
        private Text m_LanguageSettingHeader;
        [SerializeField]
        private Dropdown m_LanguageSetting;
        private int m_LanguageIndex = 0;

        [SerializeField]
        private Text m_SettingsWindowHeader;

        [SerializeField]
        private Button m_SaveButton;

        [SerializeField]
        private Button m_ReturnButton;
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.SettingsWindowUI,
                UIframework.UIManager.DisplayUIMode.HideOthers,
                UIframework.UIManager.UITypes.AboveBlurEffect,
                false, true, true);

            m_Animator = GetComponent<Animator>();
#if UNITY_EDITOR
            if (!m_Animator) Debug.LogError("Animator is null.");
            if (!m_LanguageSettingHeader) Debug.LogError("Language settings header is null.");
            if (!m_LanguageSetting) Debug.LogError("Language settings is null.");
            if (!m_SettingsWindowHeader) Debug.LogError("Settings window header is null.");
            if (!m_SaveButton) Debug.LogError("Save button is null");
            if (!m_ReturnButton) Debug.LogError("Return button is null");
#endif
            m_SaveButton.onClick.AddListener(delegate { OnClickedSaveButton(); });
            m_ReturnButton.onClick.AddListener(delegate { OnClickedReturnButton(); });

            #region language settings dropdown menu
            if (m_LanguageSetting)
            {
                foreach (var item in Enum.GetValues(typeof(GameLanguageManager.SupportedLanguage)))
                {
                    m_LanguageSetting.options.Add(new Dropdown.OptionData() {
                        text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Sett_LanguageCategory, (GameLanguageManager.SupportedLanguage)item) });
                }

                // We need this step to refresh the options list.
                m_LanguageSetting.value = 1;
                m_LanguageSetting.value = 0;

                // Set default values.
                m_LanguageIndex = 0;

                // Add event listener to the drop down menu.
                m_LanguageSetting.onValueChanged.AddListener((value) => OnSelectedLanguage(value));
            }
            #endregion

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        private void OnSelectedLanguage(int index) {
            m_LanguageIndex = index;
        }

        private void OnClickedSaveButton() {
        }

        private void OnClickedReturnButton()
        {
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
            m_SettingsWindowHeader.text = 
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Sett_SettingsWindowHeader, language);

            m_LanguageSettingHeader.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Sett_LanguageCategoryHeader, language);

            m_SaveButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Sett_SettingsWindowSaveButton, language);

            m_ReturnButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Sett_SettingsWindowReturnButton, language);
        }
    }
}


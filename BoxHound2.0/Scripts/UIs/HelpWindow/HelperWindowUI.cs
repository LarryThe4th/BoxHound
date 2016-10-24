using UnityEngine;
using UnityEngine.UI;
using BoxHound.UIframework;
using BoxHound.Utility;
using System;
using System.Collections.Generic;

namespace BoxHound.UI {
    [RequireComponent(typeof(Animator))]
    public class HelperWindowUI : UIBase
    {
        #region Public variable
        public GameObject HelpInfoButtonPrefab;
        #endregion

        #region Private variable
        // ----------------- Private variable ------------------
        [SerializeField]
        private Text m_HelperWindowHeaderText;

        [SerializeField]
        private Dropdown m_HelpInfoCategory;

        private int m_CurrentSelectedCategoryIndex = 0;

        [SerializeField]
        private Text m_HelperInfoHeader;

        private List<HelpInfoButtonUI> m_ButtonList = new List<HelpInfoButtonUI>();

        [SerializeField]
        private Text m_HelpInfoCategroyTip;

        [SerializeField]
        private Image m_HelperInfoImage;

        [SerializeField]
        private Text m_HelperInfoContext;

        [SerializeField]
        private Button m_ReturnButton;

        [SerializeField]
        private Animator m_Animator;

        private readonly string m_AnimatorParameterKeyword = "ShowWindow";

        [SerializeField]
        private RectTransform m_HelpInfoListRoot;

        [SerializeField]
        private int m_HelpInfoButtonCount = 20;
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIManager.SceneUIs.HelperWindowUI, 
                UIManager.DisplayUIMode.HideOthers, 
                UIManager.UITypes.AboveBlurEffect, 
                false, true, true);

            #region Help info category dropdown menu
            if (m_HelpInfoCategory)
            {
                foreach (var item in Enum.GetValues(typeof(HelperInfoManager.HelpInfoCategory))) {
                    m_HelpInfoCategory.options.Add(new Dropdown.OptionData() { text = HelperInfoManager.GetCategroyText((HelperInfoManager.HelpInfoCategory)item)});
                }

                // We need this step to refresh the options list.
                m_HelpInfoCategory.value = 1;
                m_HelpInfoCategory.value = 0;

                // Set default values.
                m_CurrentSelectedCategoryIndex = 0;

                // Add event listener to the drop down menu.
                m_HelpInfoCategory.onValueChanged.AddListener((value) => OnSeletedHelperCategory(value));
            }
            #endregion

            #region Help info list
            // Populate the help info buttons.
            if (HelpInfoButtonPrefab)
            {

                m_ButtonList.Clear();
                for (int index = 0; index < m_HelpInfoButtonCount; index++)
                {
                    HelpInfoButtonUI button = Instantiate(HelpInfoButtonPrefab).GetComponent<HelpInfoButtonUI>();
                    GameUtility.AddChildToParent(m_HelpInfoListRoot, button.transform);
                    m_ButtonList.Add(button);
                    button.Init(index);
                }

                OnUpdateHelperInfoButtonList(m_CurrentSelectedCategoryIndex);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("HelpInfoButtonPrefab is NULL");
#endif
            }
            #endregion

            #region Helper info contents
#if UNITY_EDITOR
            if (!m_HelperInfoHeader) Debug.LogError("Helper info header is null");
            if (!m_HelperInfoImage) Debug.LogError("Helper info image is null");
            if (!m_HelperInfoContext) Debug.LogError("Helper info context is null");
#endif
            #endregion

            #region Window active animation
            m_Animator = GetComponent<Animator>();
#if UNITY_EDITOR
            if (!m_Animator) Debug.LogError("Animator component in " + this.gameObject.name + " is null.");
#endif
            #endregion

            if (!m_HelperWindowHeaderText)
                Debug.LogError("Header text component in " + this.gameObject.name + " is null.");

            if (!m_ReturnButton) Debug.LogError("Return button component in " + this.gameObject.name + " is null.");
            m_ReturnButton.onClick.AddListener(delegate { OnClickReturnButton(); });

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        /// <summary>
        /// Respons to the dropdown menu.
        /// </summary>
        /// <param name="index"></param>
        private void OnSeletedHelperCategory(int index) {
            m_CurrentSelectedCategoryIndex = index;
            OnUpdateHelperInfoButtonList(m_CurrentSelectedCategoryIndex);
        }

        /// <summary>
        /// Receive broadcast message form the message center.
        /// When user selected a helper category form the dropdown menu on the list, the list will 
        /// update its content base on the selected category.
        /// </summary>
        /// <param name="categoryIndex"></param>
        private void OnUpdateHelperInfoButtonList(int categoryIndex) {
            HelperInfoManager.KeyWord[] keywordList = HelperInfoManager.GetHelperInfoCategoryContent((HelperInfoManager.HelpInfoCategory)categoryIndex);

            for (int index = 0; index < m_ButtonList.Count; index++)
            {
                if (index < keywordList.Length)
                {
                    m_ButtonList[index].UpdateHelpInfoButton(HelperInfoManager.GetHeaderText(keywordList[index]));
                }
                else {
                    m_ButtonList[index].UpdateHelpInfoButton(null);
                }
            }
        }

        /// <summary>
        /// A overload method, call form outside of helper window by individual helper button in scene.
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <param name="keyWordIndex"></param>
        private void OnUpdateHelperInfoContext(int categoryIndex, int keyWordIndex) {
            // Set dropdown window to current category index.
            m_HelpInfoCategory.value = categoryIndex;
            m_CurrentSelectedCategoryIndex = categoryIndex;

            OnUpdateHelperInfoButtonList(categoryIndex);

            OnUpdateHelperInfoContext(keyWordIndex);
        }

        /// <summary>
        /// Receive broadcast message form the message center, when a helper info button clicked this method 
        /// will be call.
        /// </summary>
        /// <param name="keyWordIndex"></param>
        private void OnUpdateHelperInfoContext(int keyWordIndex) {
            HelperInfoManager.KeyWord[] keywordList = HelperInfoManager.GetHelperInfoCategoryContent((HelperInfoManager.HelpInfoCategory)m_CurrentSelectedCategoryIndex);

            if (keyWordIndex >= 0 && keyWordIndex < keywordList.Length)
            {
                m_HelperInfoHeader.text = HelperInfoManager.GetHeaderText(keywordList[keyWordIndex]);
                m_HelperInfoContext.text = HelperInfoManager.GetInfoText(keywordList[keyWordIndex]);
            }
#if UNITY_EDITOR
            else {
                Debug.LogError("keyWord index out of range");
            }
#endif
        }

        /// <summary>
        /// Respon to the return button on the helper window.
        /// </summary>
        public void OnClickReturnButton()
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

        public override void EventRegister(bool reigist)
        {
            base.EventRegister(reigist);

            if (reigist)
            {
                MessageBroadCastManager.UpdateHelperInfoContentEvent += OnUpdateHelperInfoContext;
            }
            else {
                MessageBroadCastManager.UpdateHelperInfoContentEvent -= OnUpdateHelperInfoContext;
            }
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // Set the helper window header's language display.
            m_HelperWindowHeaderText.text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Help_WindowHeaderText, language);

            // Set the helper window categroy tip's language display.
            m_HelpInfoCategroyTip.text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Help_WindowCategoryText, language);

            // Set the dropdown menu's language diaplay.
            for (int index = 0; index < m_HelpInfoCategory.options.Count; index++)
            {
                m_HelpInfoCategory.options[index].text = HelperInfoManager.GetCategroyText((HelperInfoManager.HelpInfoCategory)index);
            }

            // Reset dropdown menu.
            m_HelpInfoCategory.value = 1;
            m_HelpInfoCategory.value = 0;
            m_CurrentSelectedCategoryIndex = 0;

            // Set the helper info list's language display.
            OnUpdateHelperInfoButtonList(m_CurrentSelectedCategoryIndex);

            // Set the helper window's return button's language display.
            m_ReturnButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Help_ReturnButtonText, language);
        }
    }
}
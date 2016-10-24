using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using BoxHound.UIframework;

namespace BoxHound.UI {
    public class LoadingScreenUI : UIBase
    {
        #region Private variable
        // ------------ Private variable --------------
        [SerializeField]
        private Image m_ProgressBar;

        [SerializeField]
        private Text m_ProgessText;

        [SerializeField]
        private Text m_Tips;
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(UIManager.SceneUIs.LoadingScreenUI, 
                UIManager.DisplayUIMode.Normal, UIManager.UITypes.AboveBlurEffect, 
                false, true, true);

#if UNITY_EDITOR
            if (!m_ProgressBar) Debug.LogError("Progress bar image component is null under: " + this.gameObject.name);
            if (!m_ProgessText) Debug.LogError("Progress text component is null under: " + this.gameObject.name);
            if (!m_Tips) Debug.LogError("Tips text component is null under: " + this.gameObject.name);
#endif

            // Reset the progress bar.
            m_ProgressBar.fillAmount = 0;
            m_ProgessText.text = "0 %";
            m_Tips.text = "";

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public override void ShowUI()
        {
            base.ShowUI();

            m_CanvasGroup.alpha = 1.0f;
        }

        public override void HideUI()
        {
            base.HideUI();

            m_CanvasGroup.alpha = 0.0f;
        }

        private void UpdatemProgress(float progress) {
            m_ProgressBar.fillAmount = progress;
            m_ProgessText.text = (progress * 100) + " %";
        }

        public override void EventRegister(bool reigist)
        {
            base.EventRegister(reigist);

            if (reigist)
            {
                MessageBroadCastManager.UpdateLoadingProgressEvent += UpdatemProgress;
            }
            else {
                MessageBroadCastManager.UpdateLoadingProgressEvent -= UpdatemProgress;
            }
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_Tips.text = "";
        }
    }

}

using UnityEngine;
using UnityEngine.UI;
using BoxHound.Manager;
using System;

namespace BoxHound.UI {
    [RequireComponent(typeof(Animator))]
    public class PupupWindowUI : UIBase
    {
        #region Private Variables
        // -------------- Private variable ------------
        [SerializeField]
        private Text m_PupupWindowContext;

        [SerializeField]
        // private HelperInformationManger.Category m_Context = HelperInformationManger.Category.None;

        private Animator m_Animator;

        private bool m_Displaying = false;
        #endregion

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            if (!m_PupupWindowContext) Debug.LogError("Pupup window text component is null.");
#endif
            m_Animator = GetComponent<Animator>();
            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public void Popup() {
            if (m_Displaying) return;
            m_Displaying = true;
            if (!m_Animator) m_Animator = GetComponent<Animator>();
            m_Animator.SetBool("ShowDialog", m_Displaying);
        }

        public void PullBack() {
            if (!m_Displaying) return;
            m_Displaying = false;
            if (!m_Animator) m_Animator = GetComponent<Animator>();

            Debug.Log("Pupback");

            m_Animator.SetBool("ShowDialog", m_Displaying);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_PupupWindowContext.text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Dlog_NoRoomAvailableText, language);
        }
    }
}


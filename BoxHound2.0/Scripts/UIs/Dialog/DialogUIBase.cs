using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BoxHound.UI {

    [RequireComponent(typeof(Animator))]
    public abstract class DialogUIBase : UIBase
    {
        #region Protected variables
        // ---------------- Protected variables --------------
        [SerializeField]
        protected Text m_DialogHeader;

        [SerializeField]
        protected Text m_DialogContext;

        [SerializeField]
        protected Button m_ConfirmButton;

        [SerializeField]
        protected Button m_DenyButton;

        [SerializeField]
        protected Animator m_Animator;

        protected readonly string m_ShowDialogAnimatorKeyword = "ShowDialog";
        #endregion

        public override void InitUI()
        {
            base.InitUI();

#if UNITY_EDITOR
            if (!m_DialogHeader) Debug.LogError("Dialog header text component is null.");
            if (!m_DialogContext) Debug.LogError("Dialog context text component is null.");

            if (!m_ConfirmButton) Debug.LogError("Confirm button component is null.");
            if (!m_DenyButton) Debug.LogError("Deny button component is null.");

            if (!m_Animator) Debug.LogError("Animator component is null");
#endif

            m_Animator.GetComponent<Animator>();
            m_ConfirmButton.onClick.AddListener(delegate { OnClickedConfirmButton(); });
            m_DenyButton.onClick.AddListener(delegate { OnClickedDenyButton(); });
        }

        public override void ShowUI()
        {
            base.ShowUI();
            m_Animator.SetBool(m_ShowDialogAnimatorKeyword, true);
        }

        public override void HideUI()
        {
            base.HideUI();
            m_Animator.SetBool(m_ShowDialogAnimatorKeyword, false);
        }

        protected virtual void OnClickedConfirmButton() {
            // TODO: sound
        }


        protected virtual void OnClickedDenyButton() {
            // TODO: sound
        }
    }
}


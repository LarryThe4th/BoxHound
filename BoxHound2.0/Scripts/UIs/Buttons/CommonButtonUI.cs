using UnityEngine;
using UnityEngine.UI;
using BoxHound.UIframework;
using System;

namespace BoxHound.UI
{
    [RequireComponent(typeof(Button))]
    public class CommonButtonUI : UIBase
    {
        public UIManager.SceneUIs CallWhenClicked = UIManager.SceneUIs.None;

        [SerializeField]
        private Button m_Button;

        private void Start()
        {
            InitUI();
        }

        public override void InitUI()
        {
            base.InitUI();

            if (CallWhenClicked == UIManager.SceneUIs.None)
                Debug.LogError(this.gameObject.name + " not set its target when call.");

            m_Button.onClick.AddListener(delegate { OnClickedButton(); });
        }

        public virtual void OnClickedButton() {
            UIManager.Instance.ShowUI(CallWhenClicked);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // Empty.
        }
    }
}
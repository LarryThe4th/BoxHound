using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    public class HelpInfoButtonUI : MonoBehaviour
    {
        #region Public variables
        // -------------- Public variables ----------------
        public int Index {
            get; private set;
        }
        #endregion

        #region Private variables
        // -------------- Private variables ----------------
        private Text m_Text;
        private Button m_Button;
        #endregion

        public void Init(int index) {
            Index = index;
            m_Text = GetComponentInChildren<Text>();
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(delegate { OnClickedHelperInfoButton(); });
        }

        private void OnClickedHelperInfoButton() {
            MessageBroadCastManager.OnClickedHelperInfoButton(Index);
        }

        public void UpdateHelpInfoButton(string text = "") {
            gameObject.SetActive(!string.IsNullOrEmpty(text));
            m_Button.interactable = true;
            m_Text.text = text;
        }
    }
}

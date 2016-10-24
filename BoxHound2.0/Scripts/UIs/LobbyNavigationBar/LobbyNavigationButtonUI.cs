using System;
using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    public class LobbyNavigationButtonUI : UIBase
    {
        #region Private variable
        // ---------- Private variable -----------
        [SerializeField]
        private Button m_Button;

        // Use the decoration bar as a indicator of currenlty seleceted lobby page.
        [SerializeField]
        private Image m_DecorationBar;

        [SerializeField]
        private LobbyManager.LobbyPageCategory m_NavType = LobbyManager.LobbyPageCategory.None;
        #endregion

        public void Start()
        {
#if UNITY_EDITOR
            if (m_NavType == LobbyManager.LobbyPageCategory.None)
                Debug.LogError("The navigation category on the Lobby navigation button is not yet set.");

            if (!m_DecorationBar)
                Debug.LogError("The decoration bar under the Lobby navigation button is not yet set.");

            if (!m_Button) Debug.LogError("Couldn't find the button component under lobby navigation button UI.");
#endif

            m_Button.onClick.AddListener(delegate { OnButtonClicked(); });

            // TODO:
            // Mutilpe language supprot.
            m_Button.GetComponentInChildren<Text>().text = m_NavType.ToString();

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // DOTO: Temporary method
            switch (m_NavType) {
                case LobbyManager.LobbyPageCategory.CreateRoom:
                    m_Button.GetComponentInChildren<Text>().text = GameLanguageManager.GetText(
                        GameLanguageManager.KeyWord.NavB_CreateRoomButtonText, language);
                    break;
                case LobbyManager.LobbyPageCategory.RoomBrowser:
                    m_Button.GetComponentInChildren<Text>().text = GameLanguageManager.GetText(
                        GameLanguageManager.KeyWord.NavB_RoomBrowserButtonText, language);
                    break;
                case LobbyManager.LobbyPageCategory.LoadOut:
                    m_Button.GetComponentInChildren<Text>().text = GameLanguageManager.GetText(
                        GameLanguageManager.KeyWord.NavB_LoadOutButtonText, language);
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError("The navigation type is none.");
#endif
                    break;
            }

        }

        /// <summary>
        /// This method is for the local button event.
        /// </summary>
        private void OnButtonClicked()
        {
            // Boradcast to all the navigation buttons
            MessageBroadCastManager.OnNavigationButtonClicked(m_NavType);
        }

        /// <summary>
        /// Navigation button clicked event receiver, receive broadcast message form the DelegateAndEventManager.
        /// </summary>
        /// <param name="type"></param>
        private void OnButtonClicked(LobbyManager.LobbyPageCategory type)
        {
            // If clicked button's navigation category match with this button.
            if (m_NavType == type)
            {
                // Diable the button for pervent form user accidently click on the button multiple times.
                m_Button.interactable = false;
                // Show the decoration bar, highlight the clicked button.
                m_DecorationBar.enabled = true;
            }
            else
            {
                // Enable other buttons.
                m_Button.interactable = true;
                // Hide the decoration bar
                m_DecorationBar.enabled = false;
            }
        }

        public override void EventRegister(bool reigist)
        {
            base.EventRegister(reigist);

            if (reigist)
            {
                MessageBroadCastManager.NavigationButtonClickedEvent += OnButtonClicked;
            }
            else {
                MessageBroadCastManager.NavigationButtonClickedEvent -= OnButtonClicked;
            }
        }
    }
}

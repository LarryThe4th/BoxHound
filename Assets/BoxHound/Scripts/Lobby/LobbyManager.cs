using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Larry.BoxHound {

    public class LobbyManager : MonoBehaviour
    {
        #region Public Variables
        // -------------- Public variable -------------
        [Tooltip("The cache of text UI compoment.")]
        public Text GameVersionText;        // The cache of text UI compoment.

        public Toggle RoomBrowserToggle;
        public Toggle CreateRoomToggle;
        public Toggle ExitLobbyToggle;
        public Toggle LoadOutToggle;

        public enum Pages {
            RoomBrowser,
            CreateRoom,
            Loadout,
            ExitLobby,
        }

        public bool DisableUserInput = false;
        #endregion

        #region Private Variables 
        private static LobbyManager m_Instance;
        private Pages m_CurrentEnablePage = Pages.CreateRoom;
        private List<LobbyPageButton> m_PageButtonList = new List<LobbyPageButton>();
        private int m_CurrentEnabledToggle = 0;

        private List<LobbyPage> m_PageList = new List<LobbyPage>();
        #endregion

        #region Private methods.
        /// <summary>
        /// Use this for initialization
        /// </summary>
        private void Start()
        {
            // If the game has't connected to the server yet but the lobby scene has been loaded.
            // That means something something went wrong, load back to the main menu.
            if (!NetWorkManager.IsConnected) { SceneManager.LoadScene("MainMenu"); }

            m_Instance = this;

            #region Game version text related.
            // Apply the game version text.
            if (!GameVersionText)
            {
                foreach (var text in GetComponentsInChildren<Text>())
                {
                    if (text.tag.CompareTo("GameVersionText") == 0)
                    {
                        GameVersionText = text;
                        break;
                    }
                }
                if (!GameVersionText)
                {
                    Debug.Log("Can't find the Game version Text compoment under " + this.gameObject.name);
                    return;
                }
            }
            GameVersionText.text = NetWorkManager.Instance.GetGameVersion + " Ver";
            #endregion

            foreach (var page in GetComponentsInChildren<LobbyPage>())
            {
                m_PageList.Add(page);
            }

            foreach (var pageButton in GetComponentsInChildren<LobbyPageButton>()) {
                m_PageButtonList.Add(pageButton);
                pageButton.Selected(true);
            }

            OnClickedPageButton(Pages.RoomBrowser);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Tab) && !DisableUserInput) {
                EnableNextPage();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if (m_CurrentEnablePage == Pages.CreateRoom && !DisableUserInput) {
                    GetComponentInChildren<CreateRoom>().OnClikedCreateRoomButton();
                }
                else if (m_CurrentEnablePage == Pages.RoomBrowser && !DisableUserInput) {
                    GetComponentInChildren<RoomPreview>().OnClickedJoinRoomButton();
                }
            }   
        }

        public void EnableNextPage() {
            m_PageButtonList[m_CurrentEnabledToggle].Selected(true);
            m_CurrentEnabledToggle++;
            if (m_CurrentEnabledToggle > (m_PageList.Count - 1)) m_CurrentEnabledToggle = 0;
            m_PageButtonList[m_CurrentEnabledToggle].Selected(false);
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuSwitch);
            OnClickedPageButton(m_PageList[m_CurrentEnabledToggle].GetLobbyPage());
        }
        #endregion

        /// <summary>
        /// For the three toggle labels at the top of the main contants.
        /// PS: Form 2014 to 2016, UGUI button still can't take enum type as argument. Wow, just wow.
        /// </summary>
        /// <param name="page"></param>
        public void OnClickedPageButton(Pages enablePage) {
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuSwitch);
            foreach (var button in m_PageButtonList)
            {
                if (button.RespondTo == enablePage) {
                    button.Selected(false);
                }
                else
                    button.Selected(true);
            }

            foreach (var page in m_PageList)
            {
                if (page.GetLobbyPage() == enablePage)
                    page.ShowPage(true);
                else
                    page.ShowPage(false);
            }

            m_CurrentEnabledToggle = (int)enablePage;
        }

        #region Singleton pattern of the lobby manager
        public static LobbyManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    CreateInstance();
                }

                return m_Instance;
            }
        }
        private static void CreateInstance()
        {
            if (m_Instance == null)
            {
                GameObject manager = GameObject.Find("LobbyManager");

                if (manager == null)
                {
                    manager = new GameObject("LobbyManager");
                    manager.AddComponent<LobbyManager>();
                }

                m_Instance = manager.GetComponent<LobbyManager>();
            }
        }
        #endregion
    }
}


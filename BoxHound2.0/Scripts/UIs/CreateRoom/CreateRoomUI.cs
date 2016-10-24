using UnityEngine;
using UnityEngine.UI;
using System;

namespace BoxHound.UI {
    public class CreateRoomUI : LobbyPageUIbase
    {
        #region Public variables
        // -------------- Public variable -------------
        [Tooltip("The cache of RoomName's input field UI compoment.")]
        public InputField RoomNameInput;    // The cache of RoomName's input field UI compoment.

        public Text RoomNameInputLable;

        [Tooltip("The cache of MapSelect's Dropdown compoment.")]
        public Dropdown MapSelect;          // The cache of MapSelect's input field UI compoment.

        public Text MapSelectLable;

        [Tooltip("The cache of PlayerLimit's Dropdown compoment.")]
        public Dropdown PlayerLimit;        // The cache of input label text compoment.

        public Text PlayerLimitLable;

        [Tooltip("The cache of GameMode's Dropdown compoment.")]
        public Dropdown GameMode;           // The cache of Login button UI compoment.

        public Text GameModeLable;

        [Tooltip("The cache of TimeLimit's Dropdown compoment.")]
        public Dropdown TimeLimit;          // The cache of TimeLimit's Dropdown compoment.

        public Text TimeLimiLable;

        [Tooltip("The cache of HealthLimit's Dropdown compoment.")]
        public Dropdown HealthLimit;          // The cache of HealthLimit's Dropdown compoment.

        public Text HealthLimitLable;

        [Tooltip("The cache of CreateRoom's Button compoment.")]
        public Button CreateRoomButton;     // The cache of CreateRoom's Button compoment.

        [Tooltip("The cache of Text compoment under create room button.")]
        public Text CreateRoomButtonText;

        [Tooltip("The cache of the MapPreview's Image compoment.")]
        public Image MapPreviewImage;       // The cache of the MapPreview's Image compoment.

        public Text GameMapNamePreview; 

        //[Tooltip("The Animator compoment for create room settings fade in animation.")]
        public Animator m_Animator;
        #endregion

        #region Private Variables
        // The user inserted room name.
        private string m_RoomName = "";

        // The user selected game map scene name.
        private int m_GameMapID = 0;

        // The user selected game mode.
        private int m_GameModeID = 0;

        // The maximun player count limit.
        private byte m_PlayerLimit = 0;

        // The time limit pre round.
        private int m_TimeLimit = 0;

        // The health limit for every player pre life.
        private int m_HealthLimit = 0;

        // Flag
        private bool m_CreatingRoom = false;

        private readonly string m_AnimationParameterKeyword = "ShowPage";

        private string m_CreatingRoomText = "";
        private string m_NeedANameForCreateNewRoom = "";
        private string m_SameNameExistWarning = "";
        #endregion

        #region Private methods
        /// <summary>
        /// The method for the room name's input field.
        /// </summary>
        /// <param name="name">The name of the room.</param>
        private void SetRoomName(string name)
        {
            // If the room name is null or empty, user can't create a game.
            CreateRoomButton.interactable = string.IsNullOrEmpty(name) ? false : true;

            if (CreateRoomButton.interactable)
            {
                CreateRoomButtonText.text = "ルームを作成し入室する";
            }
            else
            {
                CreateRoomButtonText.text = "ルーム名を設定してください";
            }

            // Store user input.
            m_RoomName = name;
        }

        /// <summary>
        /// The method for the game map selecting's drop down field.
        /// </summary>
        private void SetGameMap(int index)
        {
            if (index >= 0 && index <= GameMapManager.GetMapCount)
            {
                m_GameMapID = index;
                SetMapPreview(index);
            }
#if UNITY_EDITOR
            else
                Debug.LogError("Game map manager index is out of range.");
#endif
        }

        /// <summary>
        /// The method for the player limit setting's drop down field.
        /// </summary>
        /// <param name="index"></param>
        private void SetPlayerLimit(int index)
        {
            m_PlayerLimit = CustomRoomOptions.AvaliablePlayerLimitOptions[index];
        }

        /// <summary>
        /// The method for the game mode setting's drop down field.
        /// </summary>
        private void SetGameMode(int index)
        {
            if (index >= 0 && index <= GameModeManager.GetGameModeCount)
            {
                m_GameModeID = index;
            }
#if UNITY_EDITOR
            else
                Debug.LogError("Game mode manager index is out of range.");
#endif
        }

        /// <summary>
        /// The method for the time limit setting's drop down field.
        /// </summary>
        /// <param name="index"></param>
        private void SetTimeLimit(int index)
        {
            if (index >= 0 && index < CustomRoomOptions.AvaliableTimeLimitOptions.Length)
                m_TimeLimit = CustomRoomOptions.AvaliableTimeLimitOptions[index];
#if UNITY_EDITOR
            else
                Debug.LogError("The given index ( " + index + " ) of avaliable time limit option is vaild.");
#endif
        }

        /// <summary>
        /// The method for the health limit setting's drop down field.
        /// </summary>
        /// <param name="index"></param>
        private void SetHealthLimit(int index)
        {
            if (index >= 0 && index < CustomRoomOptions.AvaliableHealthLimitOptions.Length)
                m_HealthLimit = CustomRoomOptions.AvaliableHealthLimitOptions[index];
#if UNITY_EDITOR
            else
                Debug.LogError("The given index ( " + index + " ) of avaliable health limit option is vaild.");
#endif
        }

        private void SetMapPreview(int index)
        {
            MapPreviewImage.sprite = GameMapManager.GetGameMap(index).GameMapPreviewImage;
            GameMapNamePreview.text = GameMapManager.GetGameMap(index).GameMapName;
        }

        /// <summary>
        /// Responce to the create room button clicked event.
        /// </summary>
        public void OnClikedCreateRoomButton()
        {
            // SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
            // Prevent form user accidently clicked this button multiple times.
            CreateRoomButton.interactable = false;

            // LobbyManager.Instance.EnableCurrentPageFunction(false);
            // Check if the room name is ok to be used.
            if (RoomNameCheck())
            {
                CreateRoomButtonText.text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_CreatingRoomText, GameLanguageManager.CurrentLanguage);
                // Disable all user input while creating a game room
                // LobbyManager.IgnoreUserInput = true;
                // Set room options based on the user settings.
                RoomOptions roomOption = SetRoomOptions();
                // Create room with these room options.
                NetworkManager.Instance.CreateRoom(m_RoomName, roomOption);
            }
            else
            {
                CreateRoomButtonText.text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_SameNameExistWarning, GameLanguageManager.CurrentLanguage);
            }
        }

        private RoomOptions SetRoomOptions()
        {
            RoomOptions roomOption = new RoomOptions();

            // Default settings for all the rooms.
            roomOption.MaxPlayers = m_PlayerLimit;
            roomOption.IsOpen = true;
            roomOption.IsVisible = true;

            // Custom settings for the room.
            roomOption.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

            // Map name
            roomOption.CustomRoomProperties.Add(RoomProperties.RoomName, m_RoomName);

            // Keep track when the room was created.
            roomOption.CustomRoomProperties.Add(RoomProperties.RoomCreateDate, PhotonNetwork.time);

            // Game map id as index for the GameMapList
            roomOption.CustomRoomProperties.Add(RoomProperties.MapIndex, m_GameMapID);

            // Game Mode.
            roomOption.CustomRoomProperties.Add(RoomProperties.GameModeIndex, m_GameModeID);

            // Round start time, apply the photon server time insure the value is a double.
            roomOption.CustomRoomProperties.Add(RoomProperties.RoundStartTime, PhotonNetwork.time);

            // Team A scoure.
            roomOption.CustomRoomProperties.Add(RoomProperties.TeamAScore, 0);

            // Team B scoure.
            roomOption.CustomRoomProperties.Add(RoomProperties.TeamBScore, 0);

            // Time limit.
            roomOption.CustomRoomProperties.Add(RoomProperties.RoundTimeLimit, m_TimeLimit);

            // Health limit.
            roomOption.CustomRoomProperties.Add(RoomProperties.HealthLimit, m_HealthLimit);

            // These are the properties that can be access while in lobby.
            roomOption.CustomRoomPropertiesForLobby = new string[] {
                RoomProperties.MapIndex,
                RoomProperties.GameModeIndex,
                RoomProperties.RoundTimeLimit,
                RoomProperties.HealthLimit
            };
            return roomOption;
        }

        /// <summary>
        /// Check if there is a room has a same name as user inputed.
        /// </summary>
        /// <returns>Returns TRUE when no match.</returns>
        private bool RoomNameCheck()
        {
            foreach (var room in PhotonNetwork.GetRoomList())
            {
                if (string.Compare(room.name, m_RoomName) == 0)
                {
                    //DialogBase.OnCallDialog(DialogBase.Dialogs.CreatingOrJoiningRoom, true, new string[] { "ルームを作成中、しばらくお待ちください" });
                    //NetWorkManager.OnReportClientConnectionState("入力したルーム名と同じ名前のルームが存在します、名前をかぶらないよう再設定してください", true);
                    // Reset room name input.
                    m_RoomName = "";
                    return false;
                }
            }
            return true;
        }
        #endregion

        /// <summary>
        /// Use this for initialization
        /// </summary>
        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.CreateRoomUI, 
                UIframework.UIManager.DisplayUIMode.Normal, 
                UIframework.UIManager.UITypes.UnderBlurEffect, 
                false, true);

            #region Init variables
            // The user inserted room name.
            m_RoomName = "";

            // The user selected game map scene name.
            m_GameMapID = 0;

            // The user selected game mode.
            m_GameModeID = 0;

            // The maximun player count limit.
            m_PlayerLimit = 0;

            // The time limit pre round.
            m_TimeLimit = 0;

            // The health limit for every player pre life.
            m_HealthLimit = 0;

            // Flag
            m_CreatingRoom = false;
            #endregion

            #region Room name
            if (RoomNameInput)
                RoomNameInput.onValueChanged.AddListener((Value) => SetRoomName(Value));
            #endregion

            #region Map select
            if (MapSelect)
            {
                for (int index = 0; index < GameMapManager.GetMapCount; index++)
                    MapSelect.options.Add(new Dropdown.OptionData() { text = GameMapManager.GetGameMap(index).GameMapName });

                // We need this step to refresh the options list.
                MapSelect.value = 1;
                MapSelect.value = 0;

                // Set default values.
                m_GameMapID = 0;
                SetMapPreview(0);

                // Add event listener to the drop down menu.
                MapSelect.onValueChanged.AddListener((value) => SetGameMap(value));
            }
            #endregion

            #region Player limit setting
            if (PlayerLimit)
            {
                for (int index = 0; index < CustomRoomOptions.AvaliablePlayerLimitOptions.Length; index++)
                    PlayerLimit.options.Add(new Dropdown.OptionData() { text = CustomRoomOptions.AvaliablePlayerLimitOptions[index].ToString() + " 人" });

                // We need this step to refresh the options list.
                PlayerLimit.value = 1;
                PlayerLimit.value = 0;

                // Set default value.
                m_PlayerLimit = CustomRoomOptions.AvaliablePlayerLimitOptions[0];

                // Add event listener to the drop down menu.
                PlayerLimit.onValueChanged.AddListener((value) => SetPlayerLimit(value));
            }
            #endregion

            #region Game mode setting
            if (GameMode)
            {
                for (int index = 0; index < GameModeManager.GetGameModeCount; index++)
                    GameMode.options.Add(new Dropdown.OptionData() { text = GameModeManager.GetGameModeDetail(index).GameModeName });

                // We need this step to refresh the options list.
                GameMode.value = 1;
                GameMode.value = 0;

                // Set default value.
                m_GameModeID = 0;

                // Add event listener to the drop down menu.
                GameMode.onValueChanged.AddListener((value) => SetGameMode(value));
            }
            #endregion

            #region Time limit setting
            if (TimeLimit)
            {
                for (int index = 0; index < CustomRoomOptions.AvaliableTimeLimitOptions.Length; index++)
                {
                    TimeLimit.options.Add(new Dropdown.OptionData()
                    {
                        text = CustomRoomOptions.AvaliableTimeLimitOptions[index].ToString() + " 分"
                    });
                }

                // We need this step to refresh the options list.
                TimeLimit.value = 1;
                TimeLimit.value = 0;

                // Set default value.
                m_TimeLimit = CustomRoomOptions.AvaliableTimeLimitOptions[0];

                // Add event listener to the drop down menu.
                TimeLimit.onValueChanged.AddListener((value) => SetTimeLimit(value));
            }
            #endregion

            #region Health limit setting
            if (HealthLimit)
            {
                for (int index = 0; index < CustomRoomOptions.AvaliableHealthLimitOptions.Length; index++)
                {
                    HealthLimit.options.Add(new Dropdown.OptionData()
                    {
                        text = CustomRoomOptions.AvaliableHealthLimitOptions[index].ToString() + " ポイント"
                    });
                }

                // We need this step to refresh the options list.
                HealthLimit.value = 1;
                HealthLimit.value = 0;

                // Set default value.
                m_HealthLimit = CustomRoomOptions.AvaliableHealthLimitOptions[0];

                // Add event listener to the drop down menu.
                HealthLimit.onValueChanged.AddListener((value) => SetHealthLimit(value));
            }
            #endregion

            #region Create room buttom
            if (CreateRoomButton)
            {
                CreateRoomButton.onClick.AddListener(delegate { OnClikedCreateRoomButton(); });
                CreateRoomButton.interactable = false;
                CreateRoomButton.GetComponentInChildren<Text>().text =
                    GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_NeedAName, GameLanguageManager.CurrentLanguage);
            }
            #endregion

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public override void ShowUI()
        {
            base.ShowUI();
            m_Animator.SetBool(m_AnimationParameterKeyword, true);
        }

        public override void HideUI()
        {
            base.HideUI();
            m_Animator.SetBool(m_AnimationParameterKeyword, false);
        }

        public override LobbyManager.LobbyPageCategory GetPage()
        {
            return LobbyManager.LobbyPageCategory.CreateRoom;
        }

        public override void PageDisplayContorl(LobbyManager.LobbyPageCategory type)
        {
            base.PageDisplayContorl(type);

            if (GetPage() == type)
            {
                // Empty
            }
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            RoomNameInputLable.text = 
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_RoomNameInputLable, language);

            MapSelectLable.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_MapSelectLable, language);

            PlayerLimitLable.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_PlayerLimitLable, language);

            GameModeLable.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_GameModeLable, language);

            TimeLimiLable.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_TimeLimiLable, language);

            HealthLimitLable.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.CreR_HealthLimitLabel, language);
        }
    }
}



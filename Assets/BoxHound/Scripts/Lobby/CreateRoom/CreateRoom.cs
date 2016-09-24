using System;
using UnityEngine;
using UnityEngine.UI;

namespace Larry.BoxHound
{
    public class CreateRoom : LobbyPage
    {
        #region Public variables
        // -------------- Public variable -------------
        [Tooltip("The cache of RoomName's input field UI compoment.")]
        public InputField RoomNameInput;    // The cache of RoomName's input field UI compoment.

        [Tooltip("The cache of MapSelect's Dropdown compoment.")]
        public Dropdown MapSelect;          // The cache of MapSelect's input field UI compoment.

        [Tooltip("The cache of PlayerLimit's Dropdown compoment.")]
        public Dropdown PlayerLimit;        // The cache of input label text compoment.

        [Tooltip("The cache of GameMode's Dropdown compoment.")]
        public Dropdown GameMode;           // The cache of Login button UI compoment.

        [Tooltip("The cache of TimeLimit's Dropdown compoment.")]
        public Dropdown TimeLimit;          // The cache of TimeLimit's Dropdown compoment.

        [Tooltip("The cache of ScoreLimit's Dropdown compoment.")]
        public Dropdown ScoreLimit;          // The cache of ScoreLimit's Dropdown compoment.

        [Tooltip("The cache of WinCondition's Dropdown compoment.")]
        public Dropdown WinCondition;          // The cache of WinCondition's Dropdown compoment.

        [Tooltip("The cache of CreateRoom's Button compoment.")]
        public Button CreateRoomButton;     // The cache of CreateRoom's Button compoment.

        [Tooltip("The cache of the MapPreview's Image compoment.")]
        public Image MapPreviewImage;       // The cache of the MapPreview's Image compoment.

        [Tooltip("The RectTransform compoment of the loading ring Image.")]
        public RectTransform LoadingRing;   // The RectTransform compoment of the loading ring Image.
        #endregion

        #region Private Variables
        // If the room name stay empty than randomly pick a per-set room name. 
        private string[] m_PreSetRandomRoomName = new string[] { "Come and fight!", "Kept you waiting, huh?", "WAR HAS CHANGED" };

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

        // The score limit pre round.
        private int m_ScoreLimit = 0;

        // The id of the Win Condition. 
        private int m_WinConditionID = -1;

        private bool m_CreatingRoom = false;

        // The roration euler value of the loading ring sprite image.
        private Vector3 m_RotationEuler;   
        #endregion

        #region Private methods
        /// <summary>
        /// The method for the room name's input field.
        /// </summary>
        /// <param name="name">The name of the room.</param>
        private void SetRoomName(string name) {
            m_RoomName = name;
        }

        /// <summary>
        /// The method for the game map selecting's drop down field.
        /// </summary>
        private void SetGameMap(int index) {
            if (index >= 0 && index <= GameMapManager.GetMapListCount) {
                m_GameMapID = index;
                SetMapPreviewImage(index);
            }
            else
                Debug.LogError("Game map manager index is out of range.");
        }

        /// <summary>
        /// The method for the player limit setting's drop down field.
        /// </summary>
        /// <param name="index"></param>
        private void SetPlayerLimit(int index) {
            m_PlayerLimit = GameModeManager.AvaliablePlayerLimitOptions[index];
        }

        /// <summary>
        /// The method for the game mode setting's drop down field.
        /// </summary>
        private void SetGameMode(int index) {
            if (index >= 0 && index <= GameModeManager.GetGameModeCount) {
                m_GameModeID = index;
            }
            else 
                Debug.LogError("Game mode manager index is out of range.");
        }

        /// <summary>
        /// The method for the time limit setting's drop down field.
        /// </summary>
        /// <param name="index"></param>
        private void SetTimeLimit(int index)
        {
            if (index >= 0 && index < GameModeManager.AvaliableTimeLimitOptions.Length)
                m_TimeLimit = GameModeManager.AvaliableTimeLimitOptions[index];
            else
                Debug.LogError("The given index ( " + index + " ) of avaliable time limit option is vaild.");
        }

        /// <summary>
        /// The method for the score limit setting's drop down field.
        /// </summary>
        /// <param name="index"></param>
        private void SetScoreLimit(int index)
        {
            if (index >= 0 && index < GameModeManager.AvaliableScoreLimitOptions.Length)
                m_TimeLimit = GameModeManager.AvaliableScoreLimitOptions[index];
            else
                Debug.LogError("The given index ( " + index + " ) of avaliable score limit option is vaild.");
        }

        /// <summary>
        /// The method for the WinCondition setting's drop down field.
        /// </summary>
        /// <param name="id">The id of the win condiction.</param>
        private void SetWinCondition(int id)
        {
            if (id >= 0 && id < GameModeManager.AvaliableWinConditionOptions.Length)
            {
                m_GameModeID = id;
            }
            else
                Debug.LogError("The given win condition id ( " + id + " ) is vaild.");
        }

        private void SetMapPreviewImage(int index) {
            MapPreviewImage.sprite = GameMapManager.GetGameMap(index).GameMapPreview;
        }

        public void OnClikedCreateRoomButton() {
            if (m_CreatingRoom) return;
            OnCreatingRoomUpdateUI(true);
            RoomOptions roomOption = SetRoomOptions();
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
            NetWorkManager.Instance.CreateRoom(m_RoomName, roomOption);
        }

        private RoomOptions SetRoomOptions() {
            RoomOptions roomOption = new RoomOptions();

            // Default settings for all the rooms.
            roomOption.MaxPlayers = m_PlayerLimit;
            roomOption.IsOpen = true;
            roomOption.IsVisible = true;

            // Custom settings for the room.
            roomOption.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

            // If the room name stay empty, give it a random pre-set room name
            if (string.IsNullOrEmpty(m_RoomName)) m_RoomName =
                    m_PreSetRandomRoomName[UnityEngine.Random.Range(0, m_PreSetRandomRoomName.Length - 1)];
            // Map name
            roomOption.CustomRoomProperties.Add(RoomProperties.RoomName, m_RoomName);

            // Keep track when the room was created.
            roomOption.CustomRoomProperties.Add(RoomProperties.RoomCreateDate, PhotonNetwork.time);

            // Game map id as index for the GameMapList
            roomOption.CustomRoomProperties.Add(RoomProperties.MapIndex, m_GameMapID);

            // Game Mode.
            roomOption.CustomRoomProperties.Add(RoomProperties.GameModeIndex, m_GameModeID);

            // Round start time, apply the photon server time insure the value is a double.
            roomOption.CustomRoomProperties.Add(RoomProperties.StartTime, PhotonNetwork.time);

            // Team A scoure.
            roomOption.CustomRoomProperties.Add(RoomProperties.TeamAScore, 0);

            // Team B scoure.
            roomOption.CustomRoomProperties.Add(RoomProperties.TeamBScore, 0);

            // Time limit.
            roomOption.CustomRoomProperties.Add(RoomProperties.TimeLimit, m_TimeLimit);

            // Score limit.
            roomOption.CustomRoomProperties.Add(RoomProperties.ScoreLimit, m_ScoreLimit);

            // Win condition.
            roomOption.CustomRoomProperties.Add(RoomProperties.WinCondition, m_WinConditionID);

            // These are the properties that can be access while in lobby.
            roomOption.CustomRoomPropertiesForLobby = new string[] {
                RoomProperties.MapIndex, RoomProperties.GameModeIndex,
                RoomProperties.TimeLimit, RoomProperties.ScoreLimit
            };
            return roomOption;
        }

        private void OnCreatingRoomUpdateUI(bool Creating) {
            CreateRoomButton.gameObject.SetActive(!Creating);
            LoadingRing.gameObject.SetActive(Creating);
            m_CreatingRoom = Creating;
        }
        #endregion

        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Start()
        {
            #region Room name
            if (RoomNameInput)
                RoomNameInput.onValueChanged.AddListener((Value) => SetRoomName(Value));
            #endregion

            #region Map select
            if (MapSelect)
            {
                for (int index = 0; index < GameMapManager.GetMapListCount; index++)
                    MapSelect.options.Add(new Dropdown.OptionData() { text = GameMapManager.GetGameMap(index).GameMapName });

                // We need this step to refresh the options list.
                MapSelect.value = 1;
                MapSelect.value = 0;

                // Set default values.
                m_GameMapID = 0;
                SetMapPreviewImage(0);

                // Add event listener to the drop down menu.
                MapSelect.onValueChanged.AddListener((value) => SetGameMap(value));
            }
            #endregion

            #region Player limit setting
            if (PlayerLimit) {
                for (int index = 0; index < GameModeManager.AvaliablePlayerLimitOptions.Length; index++)
                    PlayerLimit.options.Add(new Dropdown.OptionData() { text = GameModeManager.AvaliablePlayerLimitOptions[index].ToString() });

                // We need this step to refresh the options list.
                PlayerLimit.value = 1;
                PlayerLimit.value = 0;

                // Set default value.
                m_PlayerLimit = GameModeManager.AvaliablePlayerLimitOptions[0];

                // Add event listener to the drop down menu.
                PlayerLimit.onValueChanged.AddListener((value) => SetPlayerLimit(value));
            }
            #endregion

            #region Game mode setting
            if (GameMode) {
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
                for (int index = 0; index < GameModeManager.AvaliableTimeLimitOptions.Length; index++) {
                    TimeLimit.options.Add(new Dropdown.OptionData() {
                        text = GameModeManager.AvaliableTimeLimitOptions[index].ToString() + " minutes" });
                }

                // We need this step to refresh the options list.
                TimeLimit.value = 1;
                TimeLimit.value = 0;

                // Set default value.
                m_TimeLimit = GameModeManager.AvaliableTimeLimitOptions[0];

                // Add event listener to the drop down menu.
                TimeLimit.onValueChanged.AddListener((value) => SetTimeLimit(value));
            }
            #endregion

            #region Score limit setting
            if (ScoreLimit) {
                for (int index = 0; index < GameModeManager.AvaliableScoreLimitOptions.Length; index++) {
                    ScoreLimit.options.Add(new Dropdown.OptionData() {
                        text = GameModeManager.AvaliableScoreLimitOptions[index].ToString()
                    });
                }

                // We need this step to refresh the options list.
                ScoreLimit.value = 1;
                ScoreLimit.value = 0;

                m_ScoreLimit = GameModeManager.AvaliableScoreLimitOptions[0];

                // Add event listener to the drop down menu.
                ScoreLimit.onValueChanged.AddListener((value) => SetTimeLimit(value));
            }
            #endregion

            #region Win condition setting
            if (WinCondition) {
                for (int index = 0; index < GameModeManager.AvaliableWinConditionOptions.Length; index++)
                {
                    WinCondition.options.Add(new Dropdown.OptionData()
                    {
                        text = GameModeManager.AvaliableWinConditionOptions[index]
                    });
                }

                // We need this step to refresh the options list.
                WinCondition.value = 1;
                WinCondition.value = 0;

                m_WinConditionID = 0;

                // Add event listener to the drop down menu.
                WinCondition.onValueChanged.AddListener((value) => SetWinCondition(value));
            }
            #endregion

            #region Create room buttom
            if (CreateRoomButton) {
                CreateRoomButton.onClick.AddListener(delegate { OnClikedCreateRoomButton(); });
            }
            #endregion
        }

        // Update is called once per frame
        void Update()
        {
            if (m_CreatingRoom)
            {
                // Increment 30 degrees every second.
                m_RotationEuler += Vector3.forward * -90 * Time.deltaTime;
                LoadingRing.rotation = Quaternion.Euler(m_RotationEuler);
            }
        }

        public override void ShowPage(bool show)
        {
            base.ShowPage(show);
        }

        public override LobbyManager.Pages GetLobbyPage()
        {
            return LobbyManager.Pages.CreateRoom;
        }
    }
}

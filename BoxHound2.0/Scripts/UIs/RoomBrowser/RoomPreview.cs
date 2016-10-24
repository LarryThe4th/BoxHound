using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    [RequireComponent(typeof(Animator))]
    public class RoomPreview : MonoBehaviour
    {
        #region Events
        private void OnEnable()
        {
            // Subscrible to the room details class. 
            // When user selected a room it should update the preview UI.
            MessageBroadCastManager.RoomPreviewElementClickedEvent += OnUpdateRoomPreview;
        }

        private void OnDisable()
        {
            MessageBroadCastManager.RoomPreviewElementClickedEvent += OnUpdateRoomPreview;
        }
        #endregion

        #region Private variables.
        // -------------- Private variable -------------
        [Tooltip("The cache of MapPreview's Image UI compoment.")]
        [SerializeField]
        private Image MapPreviewImage;    // The image represent the game map.

        [Tooltip("The cache of MapName preview's Text UI compoment.")]
        [SerializeField]
        private Text MapNameText;        // The name of the game map.

        [Tooltip("The cache of Room name preview's Text UI compoment.")]
        [SerializeField]
        private Text RoomNameText;        // The name of the room.

        [Tooltip("The cache of GameMode preview's Text UI compoment.")]
        [SerializeField]
        private Text GameModeText;       // The game mode.

        [Tooltip("The cache of PlayerCount preview's Text UI compoment.")]
        [SerializeField]
        private Text PlayerCountLabelText;    // The number of current in-game player and the maximun player count.
        [SerializeField]
        private Text PlayerCountText;

        [Tooltip("The cache of player health limit preview's Text UI compoment.")]
        [SerializeField]
        private Text HealthLimitLabelText;    // The health limit of each player in room.
        [SerializeField]
        private Text HealthLimitText;

        [Tooltip("The cache of TimeLimit preview's Text UI compoment.")]
        [SerializeField]
        private Text TimeLimitLabelText;      // The time limit pre round.
        [SerializeField]
        private Text TimeLimitText;

        [Tooltip("The cache of ScoreLimit preview's Text UI compoment.")]
        [SerializeField]
        private Text ScoreLimitLabelText;      // The score limit pre round.
        [SerializeField]
        private Text ScoreLimitText;

        [Tooltip("The cache of join room button compoment.")]
        [SerializeField]
        private Button JoinRoomButton;

        private string m_RoomFull = "";
        private string m_Join = "";

        [Tooltip("The cache of Room Preview Animator compoment.")]
        [SerializeField]
        private Animator RoomPreviewAnimation;

        private readonly string m_AnimationParameterKeyword = "ShowPreview";

        [HideInInspector]
        public string SelectedRoomName  // The current user selected room's name.
        { get; private set; }
        public double RoomCreateDate    // The date of creation of current selected room.
        { get; private set; }

        private bool m_Displaying = false;
        #endregion

        #region Private methods.
        private void Start()
        {
            RoomPreviewAnimation = GetComponent<Animator>();
            // Regster the button on click event.
            JoinRoomButton.onClick.AddListener(delegate { OnClickedJoinRoomButton(); });
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Update the room preview UI when user selceted a room detail.
        /// </summary>
        /// <param name="soruce">The selected room detail class.</param>
        /// <param name="args">All the information you need to know about the room details.</param>
        public void OnUpdateRoomPreview(int index, string roomName)
        {
            RoomInfo roomInfo = null;
            foreach (var item in PhotonNetwork.GetRoomList())
            {
                if (item.name.CompareTo(roomName) == 0)
                {
                    roomInfo = item;
                    break;
                }
            }
            if (roomInfo == null)
            {
#if UNITY_EDITOR
                Debug.Log("Can't find such room with room name: " + roomName + " in RoomPreview.");
#endif
                return;
            }

            SetLanguage(GameLanguageManager.CurrentLanguage);

            if (roomInfo.customProperties.ContainsKey(RoomProperties.MapIndex))
            {
                MapPreviewImage.sprite = GameMapManager.GetGameMap((int)roomInfo.customProperties[RoomProperties.MapIndex]).GameMapPreviewImage;
                MapNameText.text = GameMapManager.GetGameMap((int)roomInfo.customProperties[RoomProperties.MapIndex]).GameMapName;
            }

            SelectedRoomName = roomInfo.name;
            RoomNameText.text = SelectedRoomName;

            if (roomInfo.customProperties.ContainsKey(RoomProperties.RoomCreateDate))
            {
                RoomCreateDate = (double)roomInfo.customProperties[RoomProperties.RoomCreateDate];
            }

            if (roomInfo.customProperties.ContainsKey(RoomProperties.GameModeIndex))
            {
                GameModeManager.GameModeDetail detail = GameModeManager.GetGameModeDetail((int)roomInfo.customProperties[RoomProperties.GameModeIndex]);
                GameModeText.text = detail.GameModeName + "(" + detail.GameModeForShort + ")";
            }

            if (roomInfo.playerCount < roomInfo.maxPlayers) {
                PlayerCountText.text = roomInfo.playerCount + " / " + roomInfo.maxPlayers;
            } else {
                PlayerCountText.text = "< Color = red > " + roomInfo.playerCount + " / " + roomInfo.maxPlayers + " </ Color > ";
            }

            if (roomInfo.customProperties.ContainsKey(RoomProperties.HealthLimit))
            {
                HealthLimitText.text = ((int)roomInfo.customProperties[RoomProperties.HealthLimit]).ToString();
            }

            if (roomInfo.customProperties.ContainsKey(RoomProperties.RoundTimeLimit))
            {
                TimeLimitText.text = roomInfo.customProperties[RoomProperties.RoundTimeLimit].ToString() + " " + GameLanguageManager.TimeUnit;
            }

            ScoreLimitText.text = "N/A";



            if (roomInfo.playerCount == roomInfo.maxPlayers)
            {
                JoinRoomButton.interactable = false;
                JoinRoomButton.GetComponentInChildren<Text>().text = m_RoomFull;
            }
            else
            {
                JoinRoomButton.interactable = true;
                JoinRoomButton.GetComponentInChildren<Text>().text = m_Join;
            }
        }

        public void ShowRoomPreview(bool hasAvailableRoom)
        {
            if (!RoomPreviewAnimation) RoomPreviewAnimation = GetComponent<Animator>();

            if (hasAvailableRoom && !m_Displaying)
            {
                m_Displaying = true;
                RoomPreviewAnimation.SetBool(m_AnimationParameterKeyword, m_Displaying);
            }
            else if (!hasAvailableRoom && m_Displaying)
            {
                m_Displaying = false;
                RoomPreviewAnimation.SetBool(m_AnimationParameterKeyword, m_Displaying);
            }
        }

        public void OnClickedJoinRoomButton()
        {
            //SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);

            //// DialogBase.OnCallDialog(DialogBase.Dialogs.CreatingOrJoiningRoom, true, "入室中、しばらくお待ちください");

            //NetWorkManager.Instance.JoinRoom(SelectedRoomName);
        }

        private void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            PlayerCountLabelText.text = 
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.RooP_PlayerCountLabel, language);

            HealthLimitLabelText.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.RooP_HealthLimitLabel, language);

            TimeLimitLabelText.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.RooP_TimeLimitLabel, language);

            ScoreLimitLabelText.text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Roop_ScoreLimitLabel, language);

            m_RoomFull = GameLanguageManager.GetText(GameLanguageManager.KeyWord.RooP_JoinButtonRoomFull, language);
            m_Join = GameLanguageManager.GetText(GameLanguageManager.KeyWord.RooP_JoinButtonCanJoin, language);
        }
        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class RoomPreview : MonoBehaviour
    {
        #region Public variables.
        // -------------- Public variable -------------
        [Tooltip("The cache of RoomPreviewContant's RectTransform compoment.")]
        public RectTransform RoomPreviewContant;    // The cache of RoomPreviewContant's RectTransform compoment.

        [Tooltip("The cache of NoRoomAvailableTip's RectTransform compoment.")]
        public RectTransform NoRoomAvailableTip;    // The cache of NoRoomAvailableTip's RectTransform compoment.

        [Tooltip("The cache of MapPreview's Image UI compoment.")]
        public Image MapPreviewImage;    // The image represent the game map.

        [Tooltip("The cache of MapName preview's Text UI compoment.")]
        public Text MapNameText;        // The name of the game map.

        [Tooltip("The cache of GameMode preview's Text UI compoment.")]
        public Text GameModeText;       // The game mode.

        [Tooltip("The cache of PlayerCount preview's Text UI compoment.")]
        public Text PlayerCountText;    // The number of current in-game player and the maximun player count.

        [Tooltip("The cache of TimeLimit preview's Text UI compoment.")]
        public Text TimeLimitText;      // The time limit pre round.

        public Button JoinRoomButton;

        [HideInInspector]
        public string SelectedRoomName  // The current user selected room's name.
            { get; private set; }
        public double RoomCreateDate    // The date of creation of current selected room.
            { get; private set; }
        #endregion

        #region Public methods.
        /// <summary>
        /// Update the room preview UI when user selceted a room detail.
        /// </summary>
        /// <param name="soruce">The selected room detail class.</param>
        /// <param name="args">All the information you need to know about the room details.</param>
        public void OnRoomSelected(RoomDetails soruce, RoomDetailEventArgs args) {
            if (args.RoomInfo.customProperties.ContainsKey(RoomProperties.MapIndex)) {
                MapPreviewImage.sprite = GameMapManager.GetGameMap((int)args.RoomInfo.customProperties[RoomProperties.MapIndex]).GameMapPreview;
                MapNameText.text = GameMapManager.GetGameMap((int)args.RoomInfo.customProperties[RoomProperties.MapIndex]).GameMapName;
            }

            SelectedRoomName = args.RoomInfo.name;

            if (args.RoomInfo.customProperties.ContainsKey(RoomProperties.RoomCreateDate)) {
                RoomCreateDate = (double)args.RoomInfo.customProperties[RoomProperties.RoomCreateDate];
            }

            if (args.RoomInfo.customProperties.ContainsKey(RoomProperties.GameModeIndex)) {
                GameModeText.text = GameModeManager.GetGameModeDetail((int)args.RoomInfo.customProperties[RoomProperties.GameModeIndex]).GameModeName;
            }

            PlayerCountText.text = "Player      " + args.RoomInfo.playerCount + " / " + args.RoomInfo.maxPlayers;

            if (args.RoomInfo.customProperties.ContainsKey(RoomProperties.TimeLimit))
            {
                TimeLimitText.text = "Time      " + args.RoomInfo.customProperties[RoomProperties.TimeLimit].ToString() + " mintes";
            }

            if (args.RoomInfo.playerCount == args.RoomInfo.maxPlayers)
            {
                JoinRoomButton.interactable = false;
                JoinRoomButton.GetComponentInChildren<Text>().text = "Game filled";
            }
            else
            {
                JoinRoomButton.interactable = true;
                JoinRoomButton.GetComponentInChildren<Text>().text = "Join Game";
            }
        }

        public void Display(bool hasAvailableRoom)
        {
            RoomPreviewContant.gameObject.SetActive(hasAvailableRoom);
            NoRoomAvailableTip.gameObject.SetActive(!hasAvailableRoom);
        }

        public void OnClickedJoinRoomButton() {
            LobbyManager.Instance.DisableUserInput = true;
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
            NetWorkManager.Instance.JoinRoom(SelectedRoomName);
        }
        #endregion

        #region Private methods.
        private void Start()
        {
            JoinRoomButton.onClick.AddListener(delegate { OnClickedJoinRoomButton(); } );
        }

        private void OnEnable()
        {
            // Subscrible to the room details class. 
            // When user selected a room it should update the preview UI.
            RoomDetails.RoomSelected += OnRoomSelected;
        }

        private void OnDisable()
        {
            RoomDetails.RoomSelected -= OnRoomSelected;
        }
        #endregion
    }
}

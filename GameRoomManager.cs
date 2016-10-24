using UnityEngine;
using System.Collections.Generic;
using BoxHound.UIframework;

namespace BoxHound {
    public class GameRoomManager : SceneManagerBase
    {

        #region Events
        private void OnEnable() {
            MessageBroadCastManager.PlayerStartGameEvent += OnPlayerStartGame;
        }
        private void OnDisable() {
            MessageBroadCastManager.PlayerStartGameEvent -= OnPlayerStartGame;
        }
        #endregion


        #region Public Variables
        // -------------- Public variable -------------
        public GameObject PlayerPrefab;
        public static GameRoomManager Instance;

        public Camera GameSceneCamera;
        #endregion

        #region Game phase
        public enum GamePhase
        {
            // ---------------------------------------------------
            InitializeGame,     // When the map scene first time loaded, initialize anything.
            // ---------------------------------------------------
            Preparation,        // The game now is initialized, 
                                // in this phase player can chose its team, switch loadout and etc.
                                // ---------------------------------------------------
            RoundStart,         // Round start right after the preparation phase.
            // ---------------------------------------------------
            RunningGame,        // Player is playing the game.
            // ---------------------------------------------------
            RoundEnded,         // The timer hits ZERO or someone accomplish the objectives, 
                                // The game ends here and waiting for reset the map scene.
                                // ---------------------------------------------------
            ToNextRound,        // Reset the current map scene or load a new map scene, 
                                // if reset than next phase is "Preparation", 
                                // else will be "InitializeGame".
        }
        // Keep tracking the loacl game progress.
        public static GamePhase CurrentPhase
        { get; private set; }
        #endregion

        #region Game Mode
        // The current running game mode.
        public static GameModeBase CurrentGameMode
        { get; private set; }
        private List<GameModeBase> m_GameModeList = new List<GameModeBase>();
        #endregion

        public override void InitScene()
        {
            Instance = this;

            #region Camera related control
            // Disable UI manager's scene camera.
            UIManager.Instance.EnableUISceneCamera(false);

            // Init scene camera.
            if (!GameSceneCamera) GameSceneCamera = GetComponentInChildren<Camera>();
            #endregion

            // Initialize all available game modes.
            InitAllAvailableGameMode();

            // Since something of the UI elements are operate based on specifiec room properity
            InitGameRoomUI();

            // Instantiate local player character game object.
            InitLocalPlayer();

            // After initiating the current selected game mode, 
            // start a new round.
            CurrentGameMode.StartNewRound(PhotonNetwork.room);

            // After finished all initiaztion, Show the Preparation UI.
            // let the player choise a team or wait until round starts.
            UIManager.Instance.GetUI(UIManager.SceneUIs.GameOpeningUI).ShowUI();
        }

        private void InitAllAvailableGameMode()
        {
            //Check if we are actually connected to a room
            if (PhotonNetwork.room == null) return;

            // Clear it so nothing goes wrong.
            m_GameModeList.Clear();

            // Currently only free for all is available :( sorry.
            GameModeManager.GameModes roomSettingGameMode = GameModeManager.GameModes.FreeForAll;
            // Get current selected game mode form room custom properties.
            if (RoomProperties.ContantsKey(PhotonNetwork.room, RoomProperties.GameModeIndex))
            {
                roomSettingGameMode = GameModeManager.GetGameModeDetail(
                    (int)PhotonNetwork.room.customProperties[RoomProperties.GameModeIndex]).GameMode;
            }
#if UNITY_EDITOR
            else { 
                Debug.LogError("Room property key GameModeIndex does not exist.");
            }   
#endif


            // Find all the available game modes.
            foreach (var gameMode in GetComponentsInChildren<GameModeBase>())
            {
                // If this game mode script matches currently selected game mode, enable the script.
                if (gameMode.GetGameMode() == roomSettingGameMode)
                {
                    CurrentGameMode = gameMode;
                    CurrentGameMode.Setup(true);
                }
                // Else disable it.
                else
                {
                    gameMode.Setup(false);
                }

                // Add available game mode into the list.
                m_GameModeList.Add(gameMode);
            }
        }

        private void InitGameRoomUI() {
            UIManager.Instance.LoadUI(UIManager.SceneUIs.GameOpeningUI);
            UIManager.Instance.LoadUI(UIManager.SceneUIs.PlayerHUDUI);
            // PlayerHUD
            // LeaderBorad
        }

        // Instantiate local player character game object.
        private void InitLocalPlayer() {
            // If the player prefab is ready to be instantiate.
            if (PlayerPrefab)
            {
                // If the local player instance has not been instantiated yet.
                if (CharacterManager.LocalPlayer == null)
                {
                    // Now we're in a room. spawn a character for the local player. 
                    // It gets synced by using PhotonNetwork.Instantiate
                    GameObject loaclPlayerObject = PhotonNetwork.Instantiate(
                        this.PlayerPrefab.name,
                        // Somewhere under ground
                        new Vector3(0f, -10f, 0f),
                        Quaternion.identity,
                        // At group ZERO, this is not about team but photon network group,
                        // now we don't need it so let it be.
                        0);

                    // Set LocalPlayerInstance.
                    CharacterManager.LocalPlayer = loaclPlayerObject.GetComponent<CharacterManager>();
                }
                // If the player prefab is exist, whatever local player has already been instantiated 
                // or not, initialize the character when new scene loaded.
                CharacterManager.LocalPlayer.Init();
            }
        }

        public void EnableSceneCamera(bool enable) {
            GameSceneCamera.enabled = enable;
        }

        private void OnPlayerStartGame() {
            CharacterManager.LocalPlayer.Respawn();
        }

        public void ProcessGamePhase(GamePhase currentPhase)
        {
            CurrentPhase = currentPhase;
            switch (currentPhase)
            {
                case GamePhase.InitializeGame:
                    break;
                case GamePhase.Preparation:
                    break;
                case GamePhase.RoundStart:
                    break;
                case GamePhase.RunningGame:
                    break;
                case GamePhase.RoundEnded:
                    break;
                case GamePhase.ToNextRound:
                    break;
            }
        }
    }
}

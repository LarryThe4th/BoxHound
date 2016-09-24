using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Larry.BoxHound
{
    public class RoomManager : Photon.PunBehaviour
    {
        public enum GamePhase {
            // ---------------------------------------------------
            InitializeGame,     // When the map scene first time loaded, initialize anything.
            // ---------------------------------------------------
            Preparation,        // The game now is initialized, 
                                // in this phase player can chose its team, switch loadout and ect.
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

        #region Public Variables
        // -------------- Public variable -------------
        public GameObject PlayerPrefab;         // The prefab to use for representing the player.

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static CharacterManager LocalPlayer = null;

        public static GamePhase CurrentPhase      // Keep tracking the game progress.
            { get; private set; }

        public static GameModeBase CurrentGameMode  // The current running game mode.
            { get; private set; }    
        #endregion

        #region Private variables
        private static RoomManager m_Instance;      // The singleton design of room manager.
        private static Camera m_SceneCamera;        // Use this camera while the user is not contolling the character.
        private List<GameModeBase> m_GameModeList = new List<GameModeBase>();
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        #endregion
        /// <summary>
        /// Use this for initialization
        /// </summary>
        private void Start()
        {
            #region Safety check.
            // In case we started this Game with the wrong scene being active, 
            // simply load the main menu scene.
            if (!PhotonNetwork.connected || PhotonNetwork.room == null)
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }
            #endregion

            // When start a new map scene, is time to initialize everything.
            ProcessGamePhase(GamePhase.InitializeGame);
        }

        public void StartGame()
        {
            // Show the game info UI.
            GameUIManager.Instance.GameInfoUI.ShowUI(true);

            // Respwan player to the spwan point.
            switch (LocalPlayer.AtTeam) {
                case GameModeManager.Team.OneManArmy:
                    LocalPlayer.RespwanAndEnbaleControl(
                        CurrentGameMode.GetSpawnPoint(GameModeBase.SpawnMode.Random));
                    break;
                case GameModeManager.Team.Blue:
                    LocalPlayer.RespwanAndEnbaleControl(
                        CurrentGameMode.GetSpawnPoint(GameModeBase.SpawnMode.BlueOnly));
                    break;
                case GameModeManager.Team.Red:
                    LocalPlayer.RespwanAndEnbaleControl(
                        CurrentGameMode.GetSpawnPoint(GameModeBase.SpawnMode.RedOnly));
                    break;
            }

            EnableSceneCamera(false);
        }

        // NOT FINISHED YET!
        private void ProcessGamePhase(GamePhase currentPhase)
        {
            CurrentPhase = currentPhase;
            switch (currentPhase)
            {
                case GamePhase.InitializeGame:
                    InitializeNewScene();
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

        private void InitializeNewScene() {
            // Singleton class.
            m_Instance = this;

            // Init scene camera.
            if (!m_SceneCamera) m_SceneCamera = GetComponentInChildren<Camera>();
            EnableSceneCamera(true);

            // Initializa all available game modes.
            InitAllAvailableGameMode();

            // After initiating the current selected game mode, 
            // start a new round.
            CurrentGameMode.StartNewRound(PhotonNetwork.room);

            // Instantiate local player character game object.
            InitLocalPlayer();

            // Initialize the game UI.
            if (CurrentGameMode != null)
            {
                switch (CurrentGameMode.GetGameMode())
                {
                    case GameModeManager.GameModes.FreeForAll:
                        GameUIManager.Instance.InitAllUI(false);
                        break;
                    case GameModeManager.GameModes.TeamDeathMatch:
                        GameUIManager.Instance.InitAllUI(true);
                        break;
                }
            }

            // After the initializtion is finished, switch to the Preparation phase.
            ProcessGamePhase(GamePhase.Preparation);
        }

        private void EnableSceneCamera(bool enable) {
            m_SceneCamera.gameObject.SetActive(enable);
        }

        private void InitLocalPlayer() {
            // If the player prefab is ready to be instantiate.
            if (PlayerPrefab)
            {
                // If the local player instance has not been instantiated yet.
                if (LocalPlayer == null)
                {
                    Debug.Log("Init Local Player");

                    // Now we're in a room. spawn a character for the local player. 
                    // It gets synced by using PhotonNetwork.Instantiate
                    GameObject loaclPlayer = PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0f, 20f, 0f), Quaternion.identity, 0);

                    // Set LocalPlayerInstance.
                    LocalPlayer = loaclPlayer.GetComponent<CharacterManager>();

                    // Do the initialization process for the local player.
                    LocalPlayer.Init();
                }
                else
                {
                    Debug.Log("Ignoring scene load for " + SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        private void InitAllAvailableGameMode() {
            //Check if we are actually connected to a room
            if (PhotonNetwork.room == null) return;

            // Clear it so nothing goes wrong.
            m_GameModeList.Clear();

            GameModeManager.GameModes roomSettingGameMode = GameModeManager.GameModes.FreeForAll;
            // Get current selected game mode form room custom properties.
            if (RoomProperties.CheckIfRoomContantsKey(PhotonNetwork.room, RoomProperties.GameModeIndex))
            {
                roomSettingGameMode = GameModeManager.GetGameModeDetail(
                    (int)PhotonNetwork.room.customProperties[RoomProperties.GameModeIndex]).GameMode;
            }
            else {
                Debug.LogError("Room property key GameModeIndex does not exist.");
                return;
            }

            // Find all the game modes.
            foreach (var gameMode in GetComponentsInChildren<GameModeBase>())
            {
                // If this game mode script is the one currently selected, enable the script.
                if (gameMode.GetGameMode() == roomSettingGameMode)
                {
                    CurrentGameMode = gameMode;
                    CurrentGameMode.Setup(true);
                }
                // Else disable it.
                else {
                    gameMode.Setup(false);
                }

                // Add available game mode into the list.
                m_GameModeList.Add(gameMode);
            }
        }

        #region Singleton pattern of the network manager
        public static RoomManager Instance
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
                GameObject manager = GameObject.Find("RoomManager");

                if (manager == null)
                {
                    manager = new GameObject("RoomManager");
                    manager.AddComponent<RoomManager>();
                }

                m_Instance = manager.GetComponent<RoomManager>();
            }
        }
        #endregion
    }
}

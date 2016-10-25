using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using BoxHound.UIframework;
using BoxHound.UI;
using System;

namespace BoxHound
{
    [Obsolete("Use GameRoomManager instead")]
    public class RoomManager : SceneManagerBase
    {
        #region Public Variables
        // -------------- Public variable -------------
        // The prefab to use for representing the player.
        public GameObject PlayerPrefab;

        public static RoomManager Instance; 

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

        public static GamePhase CurrentPhase      // Keep tracking the game progress.
        { get; private set; }

        public static GameModeBase CurrentGameMode  // The current running game mode.
        { get; private set; }

        public static bool GameIsPaused
        { get; private set; }
        #endregion

        #region Private variables
        /// <summary>
        /// Use this camera while the user is not contolling the character.
        /// </summary>
        private Camera SceneCamera;

        private List<GameModeBase> m_GameModeList = new List<GameModeBase>();
        #endregion

        public override void InitScene()
        {
            Instance = this;

            // TODO: remove this
            SceneCamera = GameObject.FindGameObjectWithTag("SceneCamera").GetComponent<Camera>();

            m_GameModeList = new List<GameModeBase>();
            GameIsPaused = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            CurrentPhase = GamePhase.InitializeGame;

            // When start a new map scene, is time to initialize everything.
            ProcessGamePhase(GamePhase.InitializeGame);

            // After the initializtion is finished, switch to the Preparation phase.
            ProcessGamePhase(GamePhase.Preparation);
        }

        ///// <summary>
        ///// Use this for initialization
        ///// </summary>
        //private void Start() {
        //    #region Safety check.
        //    // In case we started this Game with the wrong scene being active, 
        //    // simply load the main menu scene.
        //    if (!PhotonNetwork.connected || PhotonNetwork.room == null)
        //    {
        //        SceneManager.LoadSceneAsync("MainMenu");
        //        return;
        //    }
        //    #endregion

        //}

        // NOT FINISHED YET!
        public void ProcessGamePhase(GamePhase currentPhase)
        {
            CurrentPhase = currentPhase;
            switch (currentPhase)
            {
                case GamePhase.InitializeGame:
                    InitializeNewScene();
                    break;
                case GamePhase.Preparation:
                    RoundPreparation();
                    break;
                case GamePhase.RoundStart:
                    StartRound();
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case GamePhase.RunningGame:
                    break;
                case GamePhase.RoundEnded:
                    EnableSceneCamera(true);

                    // RoomUIManager.Instance.RoomMenuUI.DisplayUI(false);

                    CharacterManager.LocalPlayer.EnableMainCamera(false);
                    CharacterManager.LocalPlayer.EnableCharacterControl(false);
                    CharacterManager.LocalPlayer.EnableWeaponContorl(false);

                    // Hide the health bar and weapon info
                    GameRoomUI.Instance.PlayerHUDUI.DisplayWeaponInfoAndHeathBar(false);

                    // Show leader board.
                    // RoomUIManager.Instance.leaderBoardUI.DisplayUI(true);

                    // Stop Room UI form further updating
                    GameRoomUI.Instance.StopUpdateUI(true);

                    CurrentPhase = GamePhase.ToNextRound;
                    break;
                case GamePhase.ToNextRound:
                    break;
            }
        }

        #region Phase: InitializeGame
        private void InitializeNewScene()
        {
            // Init scene camera.
            if (!SceneCamera) SceneCamera = GetComponentInChildren<Camera>();

            EnableSceneCamera(true);

            // Initialize all available game modes.
            InitAllAvailableGameMode();

            InitAllUI();

            // After initiating the current selected game mode, 
            // start a new round.
            CurrentGameMode.StartNewRound(PhotonNetwork.room);

            // Instantiate local player character game object.
            InitLocalPlayer();
        }

        private void InitLocalPlayer()
        {
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

        private void InitAllAvailableGameMode()
        {
            //Check if we are actually connected to a room
            if (PhotonNetwork.room == null) return;

            // Clear it so nothing goes wrong.
            m_GameModeList.Clear();

            GameModeManager.GameModes roomSettingGameMode = GameModeManager.GameModes.FreeForAll;
            // Get current selected game mode form room custom properties.
            if (RoomProperties.ContantsKey(PhotonNetwork.room, RoomProperties.GameModeIndex))
            {
                roomSettingGameMode = GameModeManager.GetGameModeDetail(
                    (int)PhotonNetwork.room.customProperties[RoomProperties.GameModeIndex]).GameMode;
            }
            else
            {
                Debug.LogError("Room property key GameModeIndex does not exist.");
                return;
            }

            // Find all the game modes.
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

        private void InitAllUI() {
            UIManager.Instance.LoadUI(UIManager.SceneUIs.GameRoomUI);
        }
        #endregion

        #region Phase: Preparation
        private void RoundPreparation() {
            Cursor.lockState = CursorLockMode.Confined;
            // Show the game opening menu,
            // let the player choise a team or wait until round starts.
            // GameRoomUI.Instance.GameOpeningUI.DisplayUI(true);
        }
        #endregion
        private void StartRound()
        {
            // Respwan player to the spwan point.
            CharacterManager.LocalPlayer.Respawn();
            CurrentPhase = GamePhase.RunningGame;
        }

        /// <summary>
        /// When open a game menu or player leave the game window,
        /// the game should pause, disable the chracter control and open the game menu.
        /// </summary>
        public void OnPauseGame(bool pause) {
            GameIsPaused = pause;
            Cursor.visible = GameIsPaused;
            if (GameIsPaused)
            {
                // Display the mouse cursor in the application window when pause
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
            }

            CharacterManager.LocalPlayer.EnableCharacterControl(!GameIsPaused);
            CharacterManager.LocalPlayer.EnableWeaponContorl(!GameIsPaused);
            GameRoomUI.Instance.PlayerHUDUI.DisplayWeaponInfoAndHeathBar(!GameIsPaused);
            // RoomUIManager.Instance.MessageCenterUI.DisplayUI(!GameIsPaused);
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                if (GameIsPaused || CurrentPhase != GamePhase.RoundStart)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        public void EnableSceneCamera(bool enable)
        {
            if (SceneCamera)
                SceneCamera.gameObject.SetActive(enable);
            else {
                Debug.Log("Scene camera not exist");
            }
        }

        /// <summary>
        /// Check if the target team is full or not.
        /// </summary>
        /// <param name="team">The team you wanna check.</param>
        /// <returns>Return TRUE if team is full.</returns>
        public static bool CheckIfTeamFull(CustomRoomOptions.Team team)
        {
            PhotonPlayer[] playerlist = PhotonNetwork.playerList;
            int currentTeamSize = 0;
            foreach (var player in playerlist)
            {
                if (player.customProperties.ContainsKey(PlayerProperties.Team))
                {
                    // If the player's team is match with what we are looking for.
                    if (CustomRoomOptions.GetTeam((int)player.customProperties[PlayerProperties.Team]) == team)
                    {
                        currentTeamSize++;
                    }
                }
                else
                {
                    Debug.LogError("Player property TEAM is not exist.");
                    return false;
                }
            }

            if (currentTeamSize >= (PhotonNetwork.room.maxPlayers / 2))
            {
                Debug.LogError("Any one side of team's menber should not more than half of the maximun player count.");
                return false;
            }

            return currentTeamSize == PhotonNetwork.room.maxPlayers;
        }

        // If we are the master client, check if the round finish.
        // if (PhotonNetwork.isMasterClient) {

        private void Update() {
            if (CurrentPhase == GamePhase.RunningGame || 
                CurrentPhase == GamePhase.Preparation) {
                // See if round finished.
                if (CurrentGameMode.IsRoundFinished()) {
                    ProcessGamePhase(GamePhase.RoundEnded);
                }
            }
            if (CurrentPhase == GamePhase.ToNextRound) {
                CurrentGameMode.ToNextRound();
            }
        }

        protected override void EventRegister(bool reigist)
        {
           // Empty
        }
    }
}

using UnityEngine;
namespace BoxHound {
    public class NetworkManager : PunSingleton<NetworkManager>
    {
        #region Public Variables
        // -------------- Public variable -------------
        public string GetGameVersion                        // A getter of the game version.
        { get { return m_GameVersion; } }

        public static TypedLobby Lobby
        {
            get
            {
                if (m_LobbyType == null)
                {
                    m_LobbyType = new TypedLobby("BoxHoundLobby", LobbyType.Default);
                }
                return m_LobbyType;
            }
        }
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private const string m_GameVersion = "Beta 1.24";   // This client's version number. 
                                                            // Users are separated from each other by gameversion.
        private static TypedLobby m_LobbyType;              // The lobby type of the lobby when creating one.

        private static int m_MapIndexWhenCreateRoom = -1;    // Map index
        #endregion

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        private void Awake()
        {
            // Force LogLevel, only reports when error happend.
            PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;

            // Don't join the default lobby on start, we do this ourselves in OnConnectedToMaster()
            PhotonNetwork.autoJoinLobby = false;

            // This makes sure we can use PhotonNetwork.LoadLevel() on the master client,
            // and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;

            // Flag it as non-destroyable when loading new scene.
            DontDestroyOnLoad(gameObject);
        }

        #region Custom methods
        /// <summary>
        /// <para>A simple method for checking the current connection state.</para> 
        /// <para>Return Trun if connected to the master server.</para> 
        /// </summary>
        public static bool IsConnectedToServer
        {
            get
            {
                return PhotonNetwork.connected;
            }
        }

        /// <summary>
        /// Call this to start the connection process to the Photon Cloud server.
        /// </summary>
        /// <param name="userNickName">The nick name of the local player.</param>
        public void ConnectToServer(string userNickName)
        {
            // If we are in any other state than disconnected, that means we are either already
            // connected, or in the process of connecting. So we don't want to do it again
            if (PhotonNetwork.connectionState != ConnectionState.Disconnected) return;

            // Set user name.
            PhotonNetwork.player.name = userNickName;

            // First, try login to the server using the current game version.
            try
            {
                PhotonNetwork.ConnectUsingSettings(m_GameVersion);
            }
            // If failed, print out the error messeage.
            catch
            {
                // TODO Popup dialog.
                Debug.LogWarning("Couldn't connect to server.");
            }
        }

        public void CreateRoom(string roomName, RoomOptions options)
        {
            PhotonNetwork.CreateRoom(roomName, options, Lobby);
            m_MapIndexWhenCreateRoom = (int)options.CustomRoomProperties[RoomProperties.MapIndex];
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        /// <summary>
        /// Leave room method.
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// For when you are done with Photon.
        /// </summary> 
        public void DisconnectFormServer()
        {
            // If we already disconnected then no need to disconnect again.
            if (PhotonNetwork.connected == false) return;

            // Else we disconnect form the server.
            PhotonNetwork.Disconnect();
        }
        #endregion

        #region Override methods form Photon net work
        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            base.OnFailedToConnectToPhoton(cause);
        }

        /// <summary>
        /// Called when we are connected to server.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby(Lobby);
        }

        /// <summary>
        /// When we joined the lobby after connecting to Photon, we want to immediately join the demo room, or create it if it doesn't exist
        /// </summary>
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            // Load lobby scene when joined lobby.
            LoadingScreenManager.LoadScene( LoadingScreenManager.TargetScene.Lobby);
        }

        /// <summary>
        /// When we leaving the lobby
        /// </summary>
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            DisconnectFormServer();
        }


        /// <summary>
        /// Called when we successfully created a room. 
        /// </summary>
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            if (PhotonNetwork.isMasterClient && (m_MapIndexWhenCreateRoom >= 0 && m_MapIndexWhenCreateRoom <= GameMapManager.GetMapCount))
            {
                PhotonNetwork.LoadLevel(GameMapManager.GetGameMap(m_MapIndexWhenCreateRoom).GameMapSceneIndex);
                m_MapIndexWhenCreateRoom = -1;
            }
        }

        /// <summary>
        /// Called when we successfully joined a room. 
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
        }

        /// <summary>
        /// When we leaving the room, load the lobby scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
        }

        public override void OnReceivedRoomListUpdate()
        {
            base.OnReceivedRoomListUpdate();

            MessageBroadCastManager.OnRoomListUpdate();
        }


        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            base.OnPhotonPlayerConnected(newPlayer);
            // If the Unity3D upgrade its .net framework to 4.5,
            // we can do something like this:
            // PlayerConnected?.Invoke(newPlayer);
            // but now we can only hope ...
            // if (PlayerConnected != null) PlayerConnected(newPlayer);
            MessageBroadCastManager.OnPlayerJoinRoom(newPlayer);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);
            MessageBroadCastManager.OnPlayerLeftRoom(otherPlayer);
        }

        /// <summary>
        /// When we disconnected form the photon server,
        /// always return to the main menu.
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            // Debug.Log("Disconnected form the server!");
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// Most likely all rooms are full or no rooms are available.
        /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
        /// </summary>
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            base.OnPhotonRandomJoinFailed(codeAndMsg);

            // Debug.Log(codeAndMsg[1].ToString());
        }
        #endregion
    }
}

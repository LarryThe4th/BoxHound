namespace BoxHound {
    public static class MessageBroadCastManager {

        public delegate void NavigationButtonClicked(LobbyManager.LobbyPageCategory category);
        public static NavigationButtonClicked NavigationButtonClickedEvent;
        public static void OnNavigationButtonClicked(LobbyManager.LobbyPageCategory category)
        {
            if (NavigationButtonClickedEvent != null) NavigationButtonClickedEvent(category);
        }

        public delegate void RoomPreviewElementClicked(int index, string roomName);
        public static RoomPreviewElementClicked RoomPreviewElementClickedEvent;
        public static void OnRoomPreviewElementClicked(int index, string roomName)
        {
            if (RoomPreviewElementClickedEvent != null) RoomPreviewElementClickedEvent(index, roomName);
        }

        public delegate void LoadingProgress(float progress);
        public static LoadingProgress UpdateLoadingProgressEvent;
        public static void OnLoadingProgressing(float progress)
        {
            if (UpdateLoadingProgressEvent != null) UpdateLoadingProgressEvent(progress);
        }

        public delegate void ClickedHelperInfoButton(int buttonIndex);
        public static ClickedHelperInfoButton UpdateHelperInfoContentEvent;
        public static void OnClickedHelperInfoButton(int buttonIndex)
        {
            if (UpdateHelperInfoContentEvent != null) UpdateHelperInfoContentEvent(buttonIndex);
        }

        public delegate void GameLanguageChange(GameLanguageManager.SupportedLanguage language);
        public static GameLanguageChange GameLanguageChangeEvent;
        public static void OnGameLanguageChange(GameLanguageManager.SupportedLanguage language)
        {
            if (GameLanguageChangeEvent != null) GameLanguageChangeEvent(language);
        }

        public delegate void UIdisplay(bool aboveEffect);
        public static UIdisplay AboveBlurEffectUIDisplayEvent;
        public static void OnAboveBlurEffectUIDisplay(bool aboveEffect)
        {
            if (AboveBlurEffectUIDisplayEvent != null) AboveBlurEffectUIDisplayEvent(aboveEffect);
        }

        public delegate void EnableCameraBlurEffect(bool enable);
        public static EnableCameraBlurEffect EnableCameraBlurEffectEvent;
        public static void OnEnableCameraBlurEffect(bool enable)
        {
            if (EnableCameraBlurEffectEvent != null) EnableCameraBlurEffectEvent(enable);
        }

        public delegate void NetworkRoomState(PhotonPlayer player);
        public static NetworkRoomState PlayerJoinRoomEvent;
        public static void OnPlayerJoinRoom(PhotonPlayer newPlayer)
        {
            if (PlayerJoinRoomEvent != null) PlayerJoinRoomEvent(newPlayer);
        }

        public static NetworkRoomState PlayerLeftRoomEvent;
        public static void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
        {
            if (PlayerLeftRoomEvent != null) PlayerLeftRoomEvent(otherPlayer);
        }

        public delegate void OnEvent();
        public static OnEvent LoadingFinishedEvent;
        public static void OnLoadingFinished()
        {
            if (LoadingFinishedEvent != null) LoadingFinishedEvent();
        }

        public static OnEvent RoomListUpdateEvent;
        public static void OnRoomListUpdate()
        {
            if (RoomListUpdateEvent != null) RoomListUpdateEvent();
        }

        public static OnEvent SceneFinishedInitEvent;
        public static void OnSceneFinishedInit()
        {
            if (SceneFinishedInitEvent != null) SceneFinishedInitEvent();

        }

        public static OnEvent PlayerStartGameEvent;
        public static void OnPlayerStartGame()
        {
            if (PlayerStartGameEvent != null) PlayerStartGameEvent();
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using BoxHound.Utility;
using BoxHound.UI;

namespace BoxHound.UIframework {
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// When a UI is going to diaplay on scrren, based on its DisplayUIMode some other UI have to hide, 
        /// incase we can go back to previous state, we have to remenber which UI has been hided becuase of this, 
        /// so we can re-show them after hiding current UI.
        /// </summary>
        public class UIPath
        {
            public UIBase CurrentDisplayingUI = null;
            public List<UIBase> PreviousUIs = new List<UIBase>();
        }

        #region Events
        private void OnEnable()
        {
            EventRegister(true);
        }
        private void OnDisable()
        {
            EventRegister(false);
        }
        #endregion

        #region Enums
        public enum UITypes
        {
            UnderBlurEffect = 0,
            AboveBlurEffect,
        }

        public enum DisplayUIMode
        {
            Normal,         // Show the UI normaly.
            HideOthers,     // When this UI showed, others need to hide.
            NeverHide,      // Once show this UI, it will never hide untill scene change.
            Dialog          // Only one dialog window diaplay in the same time.
        }

        public enum SceneUIs
        {
            None = 0,
            MainMenuUI,
            GameMenuUI,
            SettingsWindowUI,
            HelperWindowUI,
            AboutWindowUI,
            LobbyMenu,
            ExitGameDialogUI,
            ExitMenu,
            LobbyNavigationBarUI,
            RoomBrowserUI,
            CreateRoomUI,
            LoadOutUI,
            GameRoomUI,
            GameOpeningUI,
            PlayerHUDUI,
            DamageHUDUI,
            RespawnCountDownUI,
            InGameMessageUI,
            LeaderBoardUI,
            SingleColumnListUI,
            DoubleColumnListUI,
        }
        #endregion

        #region public variable
        // ------------------- public variable ------------------
        public Camera SceneCamera;
        public Canvas UnderBlurCanvas;
        #endregion

        #region Private variable
        // ------------------- Private variable ------------------
        /// <summary>
        /// The path to the folder which contains all the UI prefab under the resources folder.
        /// </summary>
        private readonly string m_UIResourcesFolderPath = "UIprefab/";

        /// <summary>
        /// A list of all the UIs roots in the scene.
        /// These UIs will not survive through the scene loading.
        /// </summary>
        private Dictionary<SceneUIs, UIBase> m_NormalUIs = new Dictionary<SceneUIs, UIBase>();

        /// <summary>
        /// A list of all the UIs that can survive through the scene loading.
        /// </summary>
        private Dictionary<SceneUIs, UIBase> m_UIWontDestoryThroughSceneChange = new Dictionary<SceneUIs, UIBase>();

        /// <summary>
        /// Track the UI display suqence.
        /// </summary>
        private Stack<UIPath> m_UIPathTracker = new Stack<UIPath>();

        [SerializeField]
        private Transform m_UnderBlurEffectRoot;

        [SerializeField]
        private Transform m_AboveBlurEffectRoot;

        private int m_AboveBlurEffectUIDisplayCount = 0;

        // Temp.
        // private static bool m_DisableUserInput = false;
        // ------------------------------------------------------
        #endregion

        // Init UI framework when awake.
        private void Awake()
        {
            m_NormalUIs = new Dictionary<SceneUIs, UIBase>();
            m_UIWontDestoryThroughSceneChange = new Dictionary<SceneUIs, UIBase>();
            m_AboveBlurEffectUIDisplayCount = 0;

            if (!SceneCamera) SceneCamera = GameObject.FindGameObjectWithTag("SceneCamera").GetComponent<Camera>();

#if UNITY_EDITOR
            if (!m_UnderBlurEffectRoot)
                Debug.LogError("The root of under blur effect canvas is null.");

            if (!m_AboveBlurEffectRoot)
                Debug.LogError("The root of above blur effect canvas is null.");
#endif
            DontDestroyOnLoad(this);
        }

        #region Load UI
        /// <summary>
        /// Load UI form the resources folder based on the scene UI type.
        /// </summary>
        /// <param name="ui">The type of the scene UI</param>
        /// <returns>Return NULL if load resources process failed.</returns>
        public UIBase LoadUI(SceneUIs ui, Transform parent = null)
        {
            // If the target UI is already in list, ignore the call.
            if (m_NormalUIs.ContainsKey(ui) || m_UIWontDestoryThroughSceneChange.ContainsKey(ui)) return null;

            // Load the UI prefab form the resources folder.
            UIBase uiPrefab = Resources.Load<UIBase>(GetUIPrefabPath(ui));
            // If sucessfully loaded the UI prefab form the resource folder.
            if (uiPrefab != null)
            {
                // Instantiate a new object base on this prefab.
                UIBase uiGameObject = Instantiate(uiPrefab);
                // Init the UI.
                uiGameObject.InitUI();

                // Set UI gameObject's parent transform. 
                switch (uiGameObject.Properties.GetUIType)
                {
                    case UITypes.AboveBlurEffect:
                        if (!parent) GameUtility.AddChildToParent(m_AboveBlurEffectRoot, uiGameObject.transform);
                        else GameUtility.AddChildToParent(parent, uiGameObject.transform);
                        break;
                    case UITypes.UnderBlurEffect:
                        if (!parent) GameUtility.AddChildToParent(m_UnderBlurEffectRoot, uiGameObject.transform);
                        else GameUtility.AddChildToParent(parent, uiGameObject.transform);
                        break;
                    default:
#if UNITY_EDITOR
                        Debug.LogError("Unexpected switch case in UImanager's LoadUI method.");
#endif
                        return null;
                }

                // Reset the UI rect transform.
                uiGameObject.GetComponent<RectTransform>().ExpandToMaxFormCenter();

                // Only the root UI will be store to the list.
                if (uiGameObject.Properties.IsRootUI)
                {
                    // Add the ui into appropriate list.
                    if (uiGameObject.Properties.WontDestoryWhenSceneChange)
                    {
                        m_UIWontDestoryThroughSceneChange.Add(ui, uiGameObject);
                    }
                    else
                    {
                        m_NormalUIs.Add(ui, uiGameObject);
                    }
                }

                // Reture result.
                return uiGameObject;
            }
#if UNITY_EDITOR
            Debug.Log("Failed to load the UI prefab, the path to target prefab object: " + GetUIPrefabPath(ui));
#endif
            return null;
        }

        /// <summary>
        /// Get the ui prefab object path in the resources folder.
        /// </summary>
        /// <param name="ui">The target scene UI.</param>
        /// <returns>Return path to resources folder.</returns>
        private string GetUIPrefabPath(SceneUIs ui)
        {
            return m_UIResourcesFolderPath + ui.ToString();
        }
        #endregion

        #region Destory UI
        public void ClearAfterLoadScene()
        {
            foreach (var item in m_NormalUIs)
            {
                Destroy(item.Value.gameObject);
            }

            foreach (var item in m_UIWontDestoryThroughSceneChange)
            {
                item.Value.HideUI();
            }

            // Disbale blur effect
            m_AboveBlurEffectUIDisplayCount = 0;
            MessageBroadCastManager.OnEnableCameraBlurEffect(false);

            m_NormalUIs.Clear();
            m_UIPathTracker.Clear();

            GameUtility.ClearMemory();
        }
        #endregion

        /// <summary>
        /// Update the UI path.
        /// </summary>
        /// <param name="currentUI"></param>
        public void AddUIPath(UIBase currentUI)
        {
            // First check if this UI is a root UI
            if (currentUI.Properties.IsRootUI) {
                // If this UIs display mode is HideOther
                if (currentUI.Properties.GetDisplayMode == DisplayUIMode.HideOthers) {
                    // Create a new node of path.
                    UIPath newNode = new UIPath();
                    newNode.CurrentDisplayingUI = currentUI;
                    // Check all the UIs in scene and find out which are displaying.
                    foreach (var item in m_NormalUIs)
                    {
                        // Unless the UI's display mode is NeverHide, all of them have to hide.
                        if (item.Value.Properties.GetDisplayMode != DisplayUIMode.NeverHide &&
                            item.Value != currentUI && item.Value.IsDisplaying) {
                            // Hide the UI.
                            item.Value.HideUI();
                            // Remenber which UI was hided.
                            newNode.PreviousUIs.Add(item.Value);
                        }
                    }

                    foreach (var item in m_UIWontDestoryThroughSceneChange)
                    {
                        // Unless the UI's display mode is NeverHide, all of them have to hide.
                        if (item.Value.Properties.GetDisplayMode != DisplayUIMode.NeverHide &&
                            item.Value != currentUI && item.Value.IsDisplaying)
                        {
                            // Hide the UI.
                            item.Value.HideUI();
                            // Remenber which UI was hided.
                            newNode.PreviousUIs.Add(item.Value);
                        }
                    }
                    // If nothing changes
                    m_UIPathTracker.Push(newNode);
#if UNITY_EDITOR
                    Debug.Log("New Path node count: " + m_UIPathTracker.Count + " current UI: " + currentUI);
#endif
                }
            }
        }

        /// <summary>
        /// When a UI is going to hide itself, check if need back track UI path.
        /// </summary>
        /// <param name="currentUI"></param>
        public void BackTrackUIPath(UIBase currentUI)
        {
            // If the UI path tracker is empty, don't have to do anything.
            if (m_UIPathTracker.Count == 0) return;

            // If this UI is not recoded in the tracker, ignore it.
            if (m_UIPathTracker.Peek().CurrentDisplayingUI != currentUI) return;

            // First check if this UI is a root UI
            if (currentUI.Properties.IsRootUI)
            {
                // If this UIs display mode is HideOther, that means it must have some
                // other UIs need to be re-show becuase of this UI.
                if (currentUI.Properties.GetDisplayMode == DisplayUIMode.HideOthers) {
                    // Show the previous UIs
                    foreach (var ui in m_UIPathTracker.Peek().PreviousUIs)
                    {
                        ui.ShowUI();
                    }
                }
                // Since we back tracked the path, the curret node can be deleted
                m_UIPathTracker.Pop();
#if UNITY_EDITOR
                Debug.Log("Path node count: " + m_UIPathTracker.Count);
#endif
            }
        }

        public UIBase GetUI(SceneUIs ui) {
            if (m_NormalUIs == null || m_UIWontDestoryThroughSceneChange == null) return null;

            if (m_NormalUIs.ContainsKey(ui)) return m_NormalUIs[ui];
            if (m_UIWontDestoryThroughSceneChange.ContainsKey(ui)) return m_UIWontDestoryThroughSceneChange[ui];
            return null;
        }

        public T GetUI<T>(SceneUIs ui) where T : UIBase {
            if (m_NormalUIs.ContainsKey(ui)) return (T)m_NormalUIs[ui];
            if (m_UIWontDestoryThroughSceneChange.ContainsKey(ui)) return (T)m_UIWontDestoryThroughSceneChange[ui];
            return null;
        }

        public void ShowUI(SceneUIs ui) {
            if (m_NormalUIs.ContainsKey(ui)) ShowUI(m_NormalUIs[ui]);
            if (m_UIWontDestoryThroughSceneChange.ContainsKey(ui)) ShowUI(m_UIWontDestoryThroughSceneChange[ui]);
        }

        public void ShowUI(UIBase ui) {
            if (ui.IsDisplaying) return;

            // Add to UI path.
            AddUIPath(ui);

            ui.ShowUI();
        }

        public void HideUI(SceneUIs ui) {
            if (m_NormalUIs.ContainsKey(ui)) HideUI(m_NormalUIs[ui]);
            if (m_UIWontDestoryThroughSceneChange.ContainsKey(ui)) HideUI(m_UIWontDestoryThroughSceneChange[ui]);
        }

        public void HideUI(UIBase ui) {
            if (!ui.IsDisplaying) return;

            BackTrackUIPath(ui);

            ui.HideUI();
        }

        /// <summary>
        /// Receive broadcast message form the MessageBroadCastManager.
        /// If current scene has more than one UI that should be display above the 
        /// blur effect, then enable the blur effect.
        /// </summary>
        /// <param name="display"></param>
        private void OnAboveBlurEffectUIDisplay(bool display)
        {
            if (display) m_AboveBlurEffectUIDisplayCount++;
            else {
                m_AboveBlurEffectUIDisplayCount--;
                if (m_AboveBlurEffectUIDisplayCount <= 0)
                {
                    m_AboveBlurEffectUIDisplayCount = 0;
                }
            }

            if (m_AboveBlurEffectUIDisplayCount == 0) {
                MessageBroadCastManager.OnEnableCameraBlurEffect(false);
            }
            else {
                MessageBroadCastManager.OnEnableCameraBlurEffect(true);
            }
        }

        /// <summary>
        /// Receive broadcast message form the MessageBroadCastManager.
        /// When blur effect enabled, all the UIs under the blur effect will be disabled. 
        /// </summary>
        /// <param name="enbale"></param>
        private void OnBlurEffectEnabled(bool enbale) {
            if (m_NormalUIs == null) return;
            foreach (var item in m_NormalUIs)
            {
                // If this item is under the blur effect
                if (item.Value.Properties.GetUIType == UITypes.UnderBlurEffect)
                {
                    // Enable or disbale the UI based on the blur effect state.
                    item.Value.EnableCanvasGroup(!enbale);
                }
            }
        }

        public void EnableUISceneCamera(bool enable) {
            SceneCamera.gameObject.SetActive(enable);
        }

        /// <summary>
        /// Reset the canvas's render camera. 
        /// Left the paraemter as NULL will set to the scene camera under UIManager.
        /// </summary>
        /// <param name="camera"></param>
        public void ResetUnderBlurCanvasRenderCamera(Camera camera = null) {
            if (camera == null) {
                UnderBlurCanvas.worldCamera = SceneCamera;
                EnableUISceneCamera(true);
            } 
            else UnderBlurCanvas.worldCamera = camera;
        }

        //private void OnSceneInitFinised() {
        //    if (LoadSceneManager.Instance.IsInGame) {
        //        SceneCamera.clearFlags = CameraClearFlags.Skybox;
        //        // UnderBlur.worldCamera = 
        //    } else {
        //        SceneCamera.clearFlags = CameraClearFlags.SolidColor;
        //        UnderBlur.worldCamera = SceneCamera;
        //    }
        //}

        /// <summary>
        /// Use this method to subscribe to unsubscribe event to the boardcast manager.
        /// </summary>
        /// <param name="reigist"></param>
        public void EventRegister(bool reigist)
        {
            if (reigist)
            {
                MessageBroadCastManager.AboveBlurEffectUIDisplayEvent += OnAboveBlurEffectUIDisplay;
                MessageBroadCastManager.EnableCameraBlurEffectEvent += OnBlurEffectEnabled;
                MessageBroadCastManager.LoadingFinishedEvent += ClearAfterLoadScene;
                // MessageBroadCastManager.SceneFinishedInitEvent += OnSceneInitFinised;
            }
            else
            {
                MessageBroadCastManager.AboveBlurEffectUIDisplayEvent -= OnAboveBlurEffectUIDisplay;
                MessageBroadCastManager.EnableCameraBlurEffectEvent -= OnBlurEffectEnabled;
                MessageBroadCastManager.LoadingFinishedEvent -= ClearAfterLoadScene;
                // MessageBroadCastManager.SceneFinishedInitEvent -= OnSceneInitFinised;
            }
        }

        private void Update() {
            if (SceneManagerBase.IgnoreUserInput) return;
            foreach (var item in m_NormalUIs)
            {
                item.Value.Process();
            }
            foreach (var item in m_UIWontDestoryThroughSceneChange)
            {
                item.Value.Process();
            }
        }
    }
}



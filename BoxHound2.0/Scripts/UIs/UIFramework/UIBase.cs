using UnityEngine;
using BoxHound.UIframework;

namespace BoxHound.UI {
    public class UIProperties {
        /// <summary>
        /// The type of UI.
        /// </summary>
        public UIManager.UITypes GetUIType {
            get; private set;
        }

        public UIManager.DisplayUIMode GetDisplayMode {
            get; private set;
        }

        public UIManager.SceneUIs GetSceneUI {
            get; private set;
        }

        /// <summary>
        /// Set True if this UI gameObject can survive 
        /// through the game scene switch.
        /// </summary>
        public bool WontDestoryWhenSceneChange {
            get; private set;
        }

        /// <summary>
        /// Set True if the UI is the Root of the other 
        /// UIs and its parent is the canvas. 
        /// When loading a new scene, thoes UI which flaged as RootUI 
        /// will be destory and clear form the menory.
        /// </summary>
        public bool IsRootUI {
            get; private set;
        }

        public bool AlwaysIgnoreRayCast
        {
            get; private set;
        }

        /// <summary>
        /// The constructor of the UI properties
        /// </summary>
        public UIProperties(UIManager.SceneUIs ui, UIManager.DisplayUIMode mode, UIManager.UITypes type, bool alwaysIgnoreRayCast, bool isRootUI = false, bool wontDestoryWhenSceneChange = false) {
            GetUIType = type;
            GetDisplayMode = mode;
            GetSceneUI = ui;
            AlwaysIgnoreRayCast = alwaysIgnoreRayCast;
            WontDestoryWhenSceneChange = wontDestoryWhenSceneChange;
            IsRootUI = isRootUI;
        }
    }

    public abstract class UIBase : MonoBehaviour
    {
        #region Events
        private void OnEnable() {
            EventRegister(true);
        }
        private void OnDisable()
        {
            EventRegister(false);
        }
        #endregion

        #region public variable
        // ------------------- public variable ------------------
        /// <summary>
        /// The pre-set UI properties of the UI.
        /// </summary>
        public UIProperties Properties;

        /// <summary>
        /// Is this UI currently displaying on the screen?
        /// </summary>
        public bool IsDisplaying {
            get; set;
        }
        #endregion

        #region Private variable
        // ------------------- Private variable ------------------
        [SerializeField]
        protected CanvasGroup m_CanvasGroup;
        #endregion

        public virtual void InitUI() {
            IsDisplaying = false;
#if UNITY_EDITOR
            if (!m_CanvasGroup) Debug.LogError("Can't find the canvas group component under the " + this.gameObject.name);
#endif
        }

        public virtual void ShowUI() {
            if (Properties == null) Debug.Log(this.gameObject.name);

            if (Properties.GetUIType == UIManager.UITypes.AboveBlurEffect &&
                Properties.IsRootUI) {
                MessageBroadCastManager.OnAboveBlurEffectUIDisplay(true);
            }

            IsDisplaying = true;
        }

        public virtual void HideUI() {
            if (Properties.GetUIType == UIManager.UITypes.AboveBlurEffect &&
                Properties.IsRootUI) {
                MessageBroadCastManager.OnAboveBlurEffectUIDisplay(false);
            }

            IsDisplaying = false;
        }

        public void EnableCanvasGroup(bool enable) {
            // If this UI always ignore ray cast such as Health bar or leader broads,
            // ignore the call.
            if (Properties.AlwaysIgnoreRayCast) return;
            m_CanvasGroup.blocksRaycasts = enable;
        }

        public abstract void SetLanguage(GameLanguageManager.SupportedLanguage language);

        /// <summary>
        /// Use this method to subscribe to unsubscribe event to the boardcast manager.
        /// </summary>
        /// <param name="reigist"></param>
        public virtual void EventRegister(bool reigist)
        {
            if (reigist)
            {
                MessageBroadCastManager.GameLanguageChangeEvent += SetLanguage;
            }
            else
            {
                MessageBroadCastManager.GameLanguageChangeEvent -= SetLanguage;
            }
        }

        public virtual void Process() {
            // Empty
        }
    }
}


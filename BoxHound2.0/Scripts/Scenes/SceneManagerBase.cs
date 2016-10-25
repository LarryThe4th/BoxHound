using BoxHound.UIframework;
using UnityEngine;

namespace BoxHound {
    public abstract class SceneManagerBase : MonoBehaviour
    {
        #region Events
        public void OnEnable()
        {
            EventRegister(true);
        }
        public void OnDisable()
        {
            EventRegister(false);
        }
        #endregion

        public static bool IgnoreUserInput {
            get; private set;
        }

        private void Start() {
            // If current is not connected to the server, return to the main menu.
            if (!NetworkManager.IsConnectedToServer && !LoadingScreenManager.IsMainScene)
            {
                LoadingScreenManager.LoadScene(LoadingScreenManager.TargetScene.Main);
                return;
            }

            IgnoreUserInput = false;

            InitScene();

            MessageBroadCastManager.OnSceneFinishedInit();
        }

        public abstract void InitScene();

        protected virtual void EventRegister(bool reigist) {
            // Empty
        }
    }
}


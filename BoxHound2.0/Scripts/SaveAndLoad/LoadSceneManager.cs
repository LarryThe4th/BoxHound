using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using BoxHound.UIframework;

namespace BoxHound {
    public class LoadSceneManager : Singleton<LoadSceneManager>
    {
        public enum Scenes {
            Main = 0,
            Lobby,
        }

        //[Header("Loading Visuals")]
        //public Image loadingIcon;
        //public Image loadingDoneIcon;
        //public Text loadingText;
        //public Image progressBar;
        //public Image fadeOverlay;

        [Header("Timing Settings")]
        public float WaitOnLoadEnd = 0.25f;
        public float FadeDuration = 0.25f;

        [Header("Loading Settings")]
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        public ThreadPriority loadThreadPriority;

        [Header("Other")]
        // If loading additive, link to the cameras audio listener, to avoid multiple active audio listeners
        // public AudioListener audioListener;

        private AsyncOperation m_Operation;
        private static Scene m_CurrentScene;

        private static string m_SceneToLoad = "";
        // IMPORTANT! This is the build index of your loading scene. You need to change this to match your actual scene index
        static int loadingSceneIndex = 2;


        public bool IsMainScene {
            get { return (SceneManager.GetActiveScene().name == Scenes.Main.ToString()); } 
        }

        public string GetCurrentActiveScene() {
            return SceneManager.GetActiveScene().name;
        }

        public bool IsInGame {
            get {
                return (SceneManager.GetActiveScene().name != Scenes.Main.ToString() &&
                 SceneManager.GetActiveScene().name != Scenes.Lobby.ToString()); }
        }

        public void LoadSceneDirectly(Scenes scene) {
            SceneManager.LoadScene(scene.ToString());
        }

        public void LoadSceneWithTransition(Scenes scene)
        {
            LoadSceneWithTransition(scene.ToString());
        }

        public void LoadSceneWithTransition(string sceneName)
        {
            if (sceneName == SceneManager.GetActiveScene().name) {
#if UNITY_EDITOR
                Debug.Log("Try to load the current active scene :" + sceneName);
#endif
                return;
            }

            Application.backgroundLoadingPriority = ThreadPriority.High;
            m_SceneToLoad = sceneName;
            // Remenber the scene befor loading.
            m_CurrentScene = SceneManager.GetActiveScene();
            // Show the loading screen UI
            UIManager.Instance.ShowUI( UIManager.SceneUIs.LoadingScreenUI);
            // Start loading the scene.
            StartCoroutine(LoadAsync(m_SceneToLoad));
        }

        IEnumerator LoadAsync(string sceneName)
        {
            StartOperation(sceneName);

            float currentProgress = 0f;
            // Operation does not auto-activate scene, so it's stuck at 0.9
            while (LoadingFinished() == false)
            {
                yield return null;

                if (Mathf.Approximately(m_Operation.progress, currentProgress) == false)
                {
                    currentProgress = m_Operation.progress;
                    // BroadCast the loading progress.
                    MessageBroadCastManager.OnLoadingProgressing(currentProgress);
                }
            }

            // Loading finished.
            MessageBroadCastManager.OnLoadingFinished();

            if (loadSceneMode == LoadSceneMode.Additive)
                SceneManager.UnloadScene(m_CurrentScene.name);
            else
                m_Operation.allowSceneActivation = true;

            // After everything is finished, hide the loading screen UI
            UIManager.Instance.HideUI(UIManager.SceneUIs.LoadingScreenUI);
        }

        /// <summary>
        /// Start loading scene operation in the background of the Loading screen UI
        /// </summary>
        /// <param name="sceneName">The name of the target scene.</param>
        private void StartOperation(string sceneName)
        {
            Application.backgroundLoadingPriority = loadThreadPriority;
            m_Operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

            if (loadSceneMode == LoadSceneMode.Single)
                m_Operation.allowSceneActivation = false;
        }

        /// <summary>
        /// Return turn when loading finished.
        /// </summary>
        private bool LoadingFinished()
        {
            return (loadSceneMode == LoadSceneMode.Additive && m_Operation.isDone) || 
                (loadSceneMode == LoadSceneMode.Single && m_Operation.progress >= 0.9f);
        }

        //void FadeIn()
        //{
        //    fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
        //}

        //void FadeOut()
        //{
        //    fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
        //}

        //void ShowLoadingVisuals()
        //{
        //    loadingIcon.gameObject.SetActive(true);
        //    loadingDoneIcon.gameObject.SetActive(false);

        //    progressBar.fillAmount = 0f;
        //    loadingText.text = "LOADING...";
        //}

        //void ShowCompletionVisuals()
        //{
        //    loadingIcon.gameObject.SetActive(false);
        //    loadingDoneIcon.gameObject.SetActive(true);

        //    progressBar.fillAmount = 1f;
        //    loadingText.text = "LOADING DONE";
        //}
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BoxHound { 

public class LoadingScreenManager : MonoBehaviour
{
        public enum TargetScene {
            Main = 0,
            Lobby = 1,
            BoxWorld = 2,
            LoadingScene = 3
        }

        [Header("Loading Visuals")]
        public Text loadingText;
        public Image progressBar;
        public Image fadeOverlay;

        [Header("Timing Settings")]
        public float waitOnLoadEnd = 0.25f;
        public float fadeDuration = 0.25f;

        [Header("Loading Settings")]
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        public ThreadPriority loadThreadPriority;
        [Header("Other")]
        // If loading additive, link to the cameras audio listener, to avoid multiple active audio listeners
        public AudioListener audioListener;

        AsyncOperation operation;
        Scene currentScene;

        private static int m_SceneToLoad = -1;
        private static string m_LoadingSceneIndex = "LoadingScene";

        public static bool IsMainScene {
            get { return (SceneManager.GetActiveScene().name == (TargetScene.Main).ToString()); }
        }

        public static bool IsInGame {
            get {
                return !(SceneManager.GetActiveScene().name == (TargetScene.Main).ToString() &&
                      SceneManager.GetActiveScene().name == (TargetScene.Lobby).ToString() &&
                      SceneManager.GetActiveScene().name == (TargetScene.LoadingScene).ToString());
            }
        }

        public static void LoadScene(TargetScene scene) {
            LoadScene((int)scene);
        }

        public static void LoadScene(int levelNum)
        {
            m_SceneToLoad = levelNum;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            SceneManager.LoadScene(m_LoadingSceneIndex);
        }

        void Start()
        {
            if (m_SceneToLoad < 0)
                return;
            fadeOverlay.gameObject.SetActive(true); // Making sure it's on so that we can crossfade Alpha
            currentScene = SceneManager.GetActiveScene();
            StartCoroutine(LoadAsync(m_SceneToLoad));
        }

        private IEnumerator LoadAsync(int levelNum)
        {
            ShowLoadingVisuals();

            yield return null;

            FadeIn();
            StartOperation(levelNum);

            float lastProgress = 0f;

            // operation does not auto-activate scene, so it's stuck at 0.9
            while (DoneLoading() == false)
            {
                yield return null;

                if (Mathf.Approximately(operation.progress, lastProgress) == false)
                {
                    progressBar.fillAmount = operation.progress;
                    lastProgress = operation.progress;
                }
            }

            MessageBroadCastManager.OnLoadingFinished();

            if (loadSceneMode == LoadSceneMode.Additive)
                audioListener.enabled = false;

            ShowCompletionVisuals();

            yield return new WaitForSeconds(waitOnLoadEnd);

            FadeOut();

            yield return new WaitForSeconds(fadeDuration);

            if (loadSceneMode == LoadSceneMode.Additive)
                SceneManager.UnloadScene(currentScene.name);
            else
                operation.allowSceneActivation = true;
        }

        private void StartOperation(int levelNum)
        {
            Application.backgroundLoadingPriority = loadThreadPriority;
            operation = SceneManager.LoadSceneAsync(levelNum, loadSceneMode);


            if (loadSceneMode == LoadSceneMode.Single)
                operation.allowSceneActivation = false;
        }

        private bool DoneLoading()
        {
            return (loadSceneMode == LoadSceneMode.Additive && operation.isDone) || (loadSceneMode == LoadSceneMode.Single && operation.progress >= 0.9f);
        }

        void FadeIn()
        {
            fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
        }

        void FadeOut()
        {
            fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
        }

        void ShowLoadingVisuals()
        {
            progressBar.fillAmount = 0f;
            loadingText.text = "LOADING...";
        }

        void ShowCompletionVisuals()
        {
            progressBar.fillAmount = 1f;
            loadingText.text = "LOADING DONE";
        }

    }
}
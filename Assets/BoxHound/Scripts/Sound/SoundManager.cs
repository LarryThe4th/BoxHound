using UnityEngine;
using System.Collections;
namespace Larry.BoxHound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        #region Public Variables
        // -------------- Private variable ------------
        public AudioClip MenuConfirm;
        public AudioClip MenuDeny;
        public AudioClip MenuSwitch;

        public enum UISFX {
            MenuConfirm,
            MenuDeny,
            MenuSwitch
        } 

        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private AudioSource m_AudioSource;
        private static SoundManager m_Instance;
        #endregion

        private void Start() {
            m_Instance = this;
            DontDestroyOnLoad(m_Instance);

            m_AudioSource = GetComponent<AudioSource>();
        }

        public void PlayUISFX(UISFX sfx) {
            if (!m_AudioSource) return;

            switch (sfx) {
                case UISFX.MenuConfirm:
                    m_AudioSource.PlayOneShot(MenuConfirm);
                    break;
                case UISFX.MenuDeny:
                    m_AudioSource.PlayOneShot(MenuDeny);
                    break;
                case UISFX.MenuSwitch:
                    m_AudioSource.PlayOneShot(MenuSwitch);
                    break;
            }
        }


        #region Singleton pattern of the Sound manager
        public static SoundManager Instance
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
                GameObject manager = GameObject.Find("SoundManager");

                if (manager == null)
                {
                    manager = new GameObject("SoundManager");
                    manager.AddComponent<SoundManager>();
                }

                m_Instance = manager.GetComponent<SoundManager>();
            }
        }
        #endregion

    }
}

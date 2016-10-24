using UnityEngine;
using System.Collections;

namespace BoxHound
{
    [RequireComponent(typeof(Animation), typeof(AudioSource))]
    public class KillComfirmation : MonoBehaviour
    {
        #region Private variables
        // -------------- Public variable -------------
        private Animation m_Animation;
        private AudioSource m_AudioSource;
        #endregion

        // Use this for initialization
        void Start()
        {
            m_Animation = GetComponent<Animation>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        public void PlayKillConfirmAnimation() {
            m_Animation.Play();
            m_AudioSource.pitch = UnityEngine.Random.Range(.8f, 1.2f);
            m_AudioSource.Play();
        }
    }
}

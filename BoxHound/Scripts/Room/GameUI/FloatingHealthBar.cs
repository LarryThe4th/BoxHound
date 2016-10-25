using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace BoxHound.Larry
{
    public class FloatingHealthBar : MonoBehaviour
    {
        #region Publice varibales
        // -------------- Publice variable -------------
        // The canvas group of the floating health bar
        public CanvasGroup FloatingHealthBarUI;
        // The silde image of the health bar
        public Image FloatingHealthBarImage;
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        // How long will the floating health bar last since last hit.
        private float m_FloatingHealthBarDisplayDuration = 3.0f;

        // Is the floating health bar displaying
        private bool m_IsDisplaying = false;
        #endregion

        private void Start() {
            m_FloatingHealthBarDisplayDuration = 3.0f;
            m_IsDisplaying = false;
        }

        public void ShowFloatingHealthBar(int currentHealth, int maximunHealth)
        {
            FloatingHealthBarUI.alpha = 1;

            FloatingHealthBarImage.fillAmount = ((float)currentHealth / maximunHealth);

            // If the floating health bar is already displaying, 
            // stop the fadeout process and restart it.
            if (m_IsDisplaying) 
                StopAllCoroutines();

            m_IsDisplaying = true;

            StartCoroutine(FloatingHealthBarFadeout());
        }

        private IEnumerator FloatingHealthBarFadeout()
        {
            // Wait until time's up.
            yield return new WaitForSeconds(m_FloatingHealthBarDisplayDuration);
            // While the indicator is still visible.
            while (FloatingHealthBarUI.alpha > 0)
            {
                FloatingHealthBarUI.alpha -= 0.1f;
                yield return null;
            }
            // When time's up, set displaying to false to stop the update
            m_IsDisplaying = false;
            // Hide the health bar.
            FloatingHealthBarUI.alpha = 0;
        }

        private void Update()
        {
            if (CharacterManager.LocalPlayer && m_IsDisplaying)
            {
                // BildBoard effect
                transform.LookAt(
                    transform.position +
                    CharacterManager.LocalPlayer.MainCamera.transform.rotation * Vector3.back,
                    CharacterManager.LocalPlayer.MainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BoxHound.UI {
    public class CorssHairUI : MonoBehaviour
    {
        #region Private variable
        // ---------------- Private variable ---------------
        [SerializeField]
        private Image m_CorsshairSprite;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        #endregion

        public void Init() {
#if UNITY_EDITOR
            if (!m_CorsshairSprite) Debug.Log("m_CorsshairSprite is null");
            if (!m_CanvasGroup) Debug.LogError("m_CanvasGroup is null");
#endif
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void ShowCorssHair(bool show) {
            m_CanvasGroup.alpha = show ? 1.0f : 0.0f;
        }
    }
}

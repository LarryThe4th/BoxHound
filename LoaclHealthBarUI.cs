using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI {
    [RequireComponent(typeof(Animator))]
    public class LoaclHealthBarUI : MonoBehaviour
    {
        #region Private variable
        // ---------------- Private variable ---------------
        [SerializeField]
        private Image m_HealthBarTopSprite;
        [SerializeField]
        private Text m_HealthBarPointText;

        [SerializeField]
        private Animator m_Animator;
        private readonly string m_AnimatorParameterKeyword = "ShowHealth";

        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        #endregion

        public void Init() {
#if UNITY_EDITOR
            if (!m_HealthBarTopSprite) Debug.LogError("m_HealthBarTopSprite is null");
            if (!m_HealthBarPointText) Debug.LogError("m_HealthBarPointText is null");
            if (!m_CanvasGroup) Debug.LogError("m_CanvasGroup is null");
#endif 
            m_Animator = GetComponent<Animator>();
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.ignoreParentGroups = false;

            int maxHealth = (int)PhotonNetwork.room.customProperties[RoomProperties.HealthLimit];
            UpdateHealthBar(maxHealth, maxHealth);
        }

        public void ShowHealthBar(bool show) {
            m_Animator.SetBool(m_AnimatorParameterKeyword, show);
        }

        public void UpdateHealthBar(int maximunHealth, int currentHealthPoint)
        {
            m_HealthBarPointText.text = currentHealthPoint <= 0 ? "0" : currentHealthPoint.ToString();
            m_HealthBarTopSprite.fillAmount = (float)currentHealthPoint / maximunHealth;
        }
    }
}

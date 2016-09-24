using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class GameInfo : MonoBehaviour
    {
        #region Private variables
        // -------------- Public variable -------------
        [Header("weapon info UIs")]
        [SerializeField]
        private RectTransform m_Health;         // The health bar UI.
        [SerializeField]
        private RectTransform m_AmmoCount;      // The ammo count UI.
        [SerializeField]
        private RectTransform m_Reloading;      // The Reloading UI.

        [Header("Health bar UIs")]
        [SerializeField]
        private Text HealthPoints;
        [SerializeField]
        private Image HealthBarTop;

        [Header("Ammo count UIs")]
        [SerializeField]
        private Text AmmoInClip;
        [SerializeField]
        private Text TotalLeft;
        [SerializeField]
        private Text WeaponName;
        #endregion

        public void Init() {
            ShowUI(false);
        }

        public void ShowUI(bool show) {
            this.gameObject.SetActive(show);
            m_Health.gameObject.SetActive(show);
            m_AmmoCount.gameObject.SetActive(show);
            m_Reloading.gameObject.SetActive(false);
        }

        public void OnHealthUpdate(int maximunHealth, int currentHealthPoint) {
            HealthPoints.text = currentHealthPoint.ToString();
            HealthBarTop.fillAmount = (float)currentHealthPoint / maximunHealth;
        }

        public void UpdateAmmoCountUI(bool reloading, string weaponName, int roundsInClip, int totalLeftRounds) {
            WeaponName.text = weaponName;

            if (roundsInClip == 0)
                AmmoInClip.color = Color.red;
            else if (roundsInClip > totalLeftRounds)
                AmmoInClip.color = Color.yellow;
            else
                AmmoInClip.color = Color.white;

            AmmoInClip.text = roundsInClip.ToString();

            TotalLeft.text = totalLeftRounds.ToString();

            m_Reloading.gameObject.SetActive(reloading);
            m_AmmoCount.gameObject.SetActive(!reloading);
        }
    }
}

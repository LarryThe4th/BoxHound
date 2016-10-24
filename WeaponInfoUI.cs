using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    [RequireComponent(typeof(Animator))]
    public class WeaponInfoUI : MonoBehaviour
    {
        #region Event
        private void OnEnable() {
            MessageBroadCastManager.GameLanguageChangeEvent += SetLanguage;
        }
        private void OnDisable() {
            MessageBroadCastManager.GameLanguageChangeEvent -= SetLanguage;
        }
        #endregion

        #region Private variable
        // ---------------- Private variable ---------------
        [SerializeField]
        private CanvasGroup m_AmmoCountCanvasGroup; // The ammo count UI.
        [SerializeField]
        private CanvasGroup m_AmmoStateCanvasGroup; // The Reloading UI.

        [Header("Ammo count UIs")]
        [SerializeField]
        private Text m_AmmoInClipText;
        [SerializeField]
        private Text m_TotalBulletLeft;
        [SerializeField]
        private Text m_WeaponName;
        [SerializeField]
        private Text m_AmmoStateText;

        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        private string m_ReloadingText = "";
        private string m_OutOfAmmoText = "";

        private readonly string m_AnimatorParameterKeyword = "Show";
        #endregion

        // Use this for initialization
        public void Init()
        {
#if UNITY_EDITOR
            if (!m_AmmoCountCanvasGroup) Debug.Log("m_AmmoCountCanvasGroup is null");
            if (!m_AmmoStateCanvasGroup) Debug.Log("m_AmmoStateCanvasGroup is null");
            if (!m_AmmoInClipText) Debug.Log("m_AmmoInClipText is null");
            if (!m_TotalBulletLeft) Debug.Log("m_TotalBulletLeft is null");
            if (!m_WeaponName) Debug.Log("m_WeaponName is null");
            if (!m_AmmoStateText) Debug.Log("m_AmmoStateText is null");
            if (!m_CanvasGroup) Debug.LogError("m_CanvasGroup is null");
#endif

            m_Animator = GetComponent<Animator>();
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.ignoreParentGroups = false;

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public void ShowWeaponInfo(bool show)
        {
            m_Animator.SetBool(m_AnimatorParameterKeyword, show);
        }

        public void UpdateWeaponInfo(Weapon.AmmoState state, string weaponName, int roundsInClip, int totalLeftRounds, int magazineSize, bool isDefaultWeapon = false)
        {
            m_WeaponName.text = weaponName;

            switch (state)
            {
                case Weapon.AmmoState.Normal:
                    m_AmmoCountCanvasGroup.alpha = 1;
                    m_AmmoStateCanvasGroup.alpha = 0;
                    // Show white as usualy
                    m_AmmoInClipText.color = Color.white;
                    break;
                case Weapon.AmmoState.Reloading:
                    m_AmmoCountCanvasGroup.alpha = 0;
                    m_AmmoStateCanvasGroup.alpha = 1;

                    m_AmmoStateText.text = m_ReloadingText;
                    m_AmmoStateText.color = Color.yellow;
                    break;
                case Weapon.AmmoState.ClipOutOfAmmo:
                    m_AmmoCountCanvasGroup.alpha = 1;
                    m_AmmoStateCanvasGroup.alpha = 0;
                    // Set the number to red so player can notice it.
                    m_AmmoInClipText.color = Color.red;

                    break;
                case Weapon.AmmoState.OutOfAmmo:
                    m_AmmoCountCanvasGroup.alpha = 0;
                    m_AmmoStateCanvasGroup.alpha = 1;
                    m_AmmoStateText.text = m_OutOfAmmoText;
                    m_AmmoStateText.color = Color.red;
                    break;
                default:
                    m_AmmoCountCanvasGroup.alpha = 1;
                    m_AmmoStateCanvasGroup.alpha = 0;
                    // Show white as usualy
                    m_AmmoInClipText.color = Color.yellow;
                    break;
            }

            m_AmmoInClipText.text = roundsInClip.ToString();
            if (roundsInClip > magazineSize)
                m_AmmoInClipText.color = Color.yellow;

            if (isDefaultWeapon)
                m_TotalBulletLeft.text = "∞";
            else
                m_TotalBulletLeft.text = totalLeftRounds.ToString();
        }

        public void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_ReloadingText = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Weap_ReloadingText, language);
            m_OutOfAmmoText = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Weap_OutOfAmmo, language);
        }

    }
}

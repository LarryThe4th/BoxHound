using System;
using UnityEngine;
using UnityEngine.UI;
using BoxHound.UIframework;

namespace BoxHound.UI
{
    [RequireComponent(typeof(Animator))]
    public class RespawnCountDown : UIBase
    {
        #region Private variables
        // -------------- Private variable -------------
        private float m_RespawnTimer = 0.0f;

        [SerializeField]
        private Text m_RespawnText;
        [SerializeField]
        private Image m_ProgressBar;
        [SerializeField]
        private Text m_AttackerName;
        [SerializeField]
        private Text m_MurderWeapon;
        [SerializeField]
        private Text m_MurderMessage;
        [SerializeField]
        private Text m_Tips;

        private Animator m_Animator;

        private string m_RespawningInSce = "";
        private string m_MurderWeaponText = "";

        private readonly string m_AnimatorParameterKeyword = "ShowCountDown";
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIManager.SceneUIs.RespawnCountDownUI,
                UIManager.DisplayUIMode.Normal,
                UIManager.UITypes.AboveBlurEffect, 
                false, true);

            m_RespawnTimer = 0.0f;
            m_Animator = GetComponent<Animator>();

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public void ShowRespawnCountDown(string attackerName, string weaponName) {
            // Reset the respawn time.
            m_RespawnTimer = GameRoomManager.CurrentGameMode.GetRespawnTime;

            m_RespawnText.text = m_RespawningInSce + " " + m_RespawnTimer + "...";

            // Set the attack's name.
            m_AttackerName.text = attackerName;

            // Set the murder weapon's name.
            m_MurderWeapon.text = m_MurderWeaponText + " " + weaponName + "...";

            // Set the progress bar to ZERO.
            m_ProgressBar.fillAmount = 0;

            // Show UI.
            UIManager.Instance.ShowUI(this);
        }

        public override void ShowUI()
        {
            base.ShowUI();

            m_Animator.SetBool(m_AnimatorParameterKeyword, true);
        }

        public override void HideUI()
        {
            base.HideUI();

            m_Animator.SetBool(m_AnimatorParameterKeyword, false);
        }

        public void UpdateRespawnCountDown() {
            m_RespawnTimer -= Time.deltaTime;
            m_RespawnText.text = m_RespawningInSce + " " + Mathf.RoundToInt(m_RespawnTimer) + "...";
            m_ProgressBar.fillAmount = (GameRoomManager.CurrentGameMode.GetRespawnTime - m_RespawnTimer) / GameRoomManager.CurrentGameMode.GetRespawnTime;

            // If the timer hits ZERO
            if (m_RespawnTimer <= 0) {
                // Hide the UI.
                UIManager.Instance.HideUI(this);
            }
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_RespawningInSce = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Resp_RespawningInSce, language);
            m_MurderWeaponText = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Resp_MurderWeaponText, language);

        }

        public override void Process()
        {
            base.Process();
            if (IsDisplaying) {
                UpdateRespawnCountDown();
            }
        }

    }
}

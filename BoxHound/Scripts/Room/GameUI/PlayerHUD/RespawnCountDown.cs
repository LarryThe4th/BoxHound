using UnityEngine;
using UnityEngine.UI;
namespace BoxHound.UI
{
    public class RespawnCountDown : RoomUIBase
    {
        #region Publice varibales
        // -------------- Publice variable -------------
        public Text RespawnText;
        public Image ProgressBar;

        public Text AttackerName;
        public Text MurderWeapon;
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        private float m_RespawnTimer = 0.0f;
        #endregion

        private void Start() {
            m_RespawnTimer = 0.0f;
        }

        public void ShowRespawnCountDown(string attackerName, string weaponName) {
            // Reset the respawn time.
            m_RespawnTimer = RoomManager.CurrentGameMode.GetRespawnTime;

            RespawnText.text = "Respawning in " + m_RespawnTimer + "...";

            // Set the attack's name.
            AttackerName.text = attackerName;

            // Set the murder weapon's name.
            MurderWeapon.text = "with a " + weaponName + "...";

            // Set the progress bar to ZERO.
            ProgressBar.fillAmount = 0;

            // Show UI.
            DisplayUI(true);
        }

        public void UpdateRespawnCountDown() {
            m_RespawnTimer -= Time.deltaTime;
            RespawnText.text = "Respawning in " + Mathf.RoundToInt(m_RespawnTimer) + "...";
            ProgressBar.fillAmount = (RoomManager.CurrentGameMode.GetRespawnTime - m_RespawnTimer) / RoomManager.CurrentGameMode.GetRespawnTime;

            // If the timer hits ZERO
            if (m_RespawnTimer <= 0) {
                // Hide the UI.
                DisplayUI(false);
            }
        }

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            // Temp
            return GameRoomUI.RoomUITypes.PlayerHUD;
        }
    }
}

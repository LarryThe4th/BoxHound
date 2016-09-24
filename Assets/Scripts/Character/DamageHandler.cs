using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    [RequireComponent(typeof(BoxCollider))]
    public class DamageHandler : Photon.PunBehaviour
    {
        #region Private variables
        // -------------- Public variable -------------
        private readonly int m_MaximunHealthPoints = 100;  // The maximun health points on this character. 
        [SerializeField]
        private int m_CurrentHealthPoints = 100;           // The current health points, if it reachs zero the character will "die".
        #endregion  

        /// <summary>
        /// A warper of the RPC call on take damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage) {
            // Call the RPC to every same object.
            photonView.RPC("SyncDamage", PhotonTargets.Others, damage);
        }

        [PunRPC]
        private void SyncDamage(int damage) {
            // Reduce current health points.
            m_CurrentHealthPoints = m_CurrentHealthPoints - damage;
            // If current health points lower than ZERO, reset it to 0.
            if (m_CurrentHealthPoints <= 0) {
                m_CurrentHealthPoints = 0;
                // DIE! DIE! DIE!
                // ...
            }

            // Update the health points UI
            GameUIManager.Instance.GameInfoUI.OnHealthUpdate(m_MaximunHealthPoints, m_CurrentHealthPoints);
        }
    }
}

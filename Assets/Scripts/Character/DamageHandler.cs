using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    [RequireComponent(typeof(BoxCollider))]
    public class DamageHandler : Photon.PunBehaviour
    {
        #region Publice varibales
        // -------------- Publice variable -------------
        public PoolObject FloatingNumberPerfab;
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        private readonly int m_MaximunHealthPoints = 100;  // The maximun health points on this character. 
        [SerializeField]
        private int m_CurrentHealthPoints = 100;           // The current health points, if it reachs zero the character will "die".

        private int m_PoolObjectID = 0;
        #endregion  

        /// <summary>
        /// Use this to initaizle
        /// </summary>
        public void Start() {
            if (FloatingNumberPerfab) {
                // Create a lot of object for pooling.
                ObjectPoolManager.Instance.CreaterPool(FloatingNumberPerfab, GameUIManager.Instance.FloatingNumberRoot, 50);
                m_PoolObjectID = FloatingNumberPerfab.GetInstanceID();
            } else
                Debug.Log("Prefab not exist, please check agian.");
        }

        /// <summary>
        /// A warper of the RPC call on take damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Vector3 hitPosition, int damage) {
            PopFloatingNumber(hitPosition, damage);
            // Call the RPC to every same object.
            photonView.RPC("SyncDamage", PhotonTargets.Others, damage);
        }

<<<<<<< HEAD
        /// <summary>
        /// This only happen in loacl client.
        /// </summary>
        private void PopFloatingNumber(Vector3 hitPosition, int damage) {
            // Get the screen position where the floating number should be appear.
            Vector2 screenPosition = 
                RoomManager.LocalPlayer.GetMainCamera.WorldToScreenPoint(hitPosition);
            ObjectPoolManager.Instance.ReuseObject(
                m_PoolObjectID, new Vector3(
                    UnityEngine.Random.Range(screenPosition.x - 20f, screenPosition.x + 20f),
                    UnityEngine.Random.Range(screenPosition.y, screenPosition.y + 20f),
                    0), Quaternion.identity,
                new object[1] { damage });
        }

=======
>>>>>>> origin/master
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

using UnityEngine;
using System.Collections;

namespace BoxHound.UI {
    public class FloatingDamageNumberUI : MonoBehaviour
    {
        #region Private variables
        // -------------- Public variable -------------
        // The perfab of the floating damage number
        public PoolObject FloatingNumberPerfab;

        [SerializeField]
        private Transform m_FloatingNumberRoot;

        // The instance ID of the pooling object.
        private int m_PoolObjectID = 0;
        #endregion

        public void Init() {
            m_PoolObjectID = 0;

            #region Initaizle the floating damage number.
            if (FloatingNumberPerfab)
            {
                // Create a lot of object for pooling.
                ObjectPoolManager.Instance.CreaterPool(FloatingNumberPerfab, m_FloatingNumberRoot, 50);
                m_PoolObjectID = FloatingNumberPerfab.GetInstanceID();
            }
            else
                Debug.Log("Prefab not exist, please check agian.");
            #endregion
        }

        /// <summary>
        /// Display the floating damgae number to inform the player how much damage has put on the target
        /// </summary>
        /// <param name="hitPosition">Where did the bullet hit in the world space.</param>
        /// <param name="damage">How much did the damage did to the target.</param>
        public void PopFloatingNumber(Vector3 hitPosition, int damage)
        {
            // Get the screen position where the floating number should be appear.
            Vector2 screenPosition =
                CharacterManager.LocalPlayer.MainCamera.WorldToScreenPoint(hitPosition);
            // Reuse the floating number object form the object pool.
            ObjectPoolManager.Instance.ReuseObject(
                m_PoolObjectID, new Vector3(
                    UnityEngine.Random.Range(screenPosition.x - 50f, screenPosition.x + 50f),
                    UnityEngine.Random.Range(screenPosition.y, screenPosition.y + 50f),
                    0), Quaternion.identity,
                new object[1] { damage });
        }
    }
}


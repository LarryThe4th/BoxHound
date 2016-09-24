using UnityEngine;
using System.Collections.Generic;
using System;

namespace Larry.BoxHound
{
    public class WeaponManager : Photon.PunBehaviour
    {
        #region Public variables
        // -------------- Public variable -------------
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        [Header("Weapons holders")]
        [Tooltip("The transform location of where the weapon will be holded.")]
        [SerializeField]
        private Transform WeaponHolderOnRight;      // The transform location of where the weapon will be holded.

        // public Transform WeaponHolderOnLeft;     // For now we only holding weapon on the right hand side.

        private List<Weapon> m_WeaponList           // The list of all available weapons in the game.
            = new List<Weapon>();

        private int m_CurrentUsingWeaponIndex = 0;  // The index of weapons in the weapon list for fast look up.

        // When viewing leaderboard or ingame menu, the weapon has to be disabled.
        private bool m_IsWeaponUsable = false;

        private readonly string m_WeaponLayer = "Weapon";
        #endregion

        #region Private methods.
        private void Start() {
            Init();
        }

        private void Init() {
            // Before finishing the initiation, disable all weapon usage.
            m_IsWeaponUsable = false;

            #region Safety check for both weapon holder.
            // Check if both weapon holder are ready.
            if (!WeaponHolderOnRight)
                Debug.LogError("Something is wrong with the weapon holder.");
            #endregion

            #region Get all the available guns form weapon holdier.
            foreach (var weapon in WeaponHolderOnRight.GetComponentsInChildren<Weapon>())
            {
                if (!m_WeaponList.Contains(weapon))
                {
                    weapon.Init(photonView);
                    m_WeaponList.Add(weapon);
                    // Set the layer
                    if (photonView.isMine) {
                        weapon.gameObject.SetLayer(LayerMask.NameToLayer(m_WeaponLayer));
                    }
                }
            }
            if (m_WeaponList.Count == 0)
                Debug.LogError("Couldn't find any weapon under the character's weapon holding point.");
            #endregion

            // Set the first weapon in the list as the starter weapon.
            // TODO: This is going to change by adding the loadout function.
            m_CurrentUsingWeaponIndex = 0;
        }

        private void Update() {
            // If the weapon is non-usable or this object is not controlled by local player,
            // don't do anything.
            if (!m_IsWeaponUsable || !photonView.isMine) return;

            // Process the current using weapon.
            m_WeaponList[m_CurrentUsingWeaponIndex].Process();
        }
        #endregion

        #region Public methods.
        public void EnableWeapon(bool enable) {
            m_IsWeaponUsable = enable;
        }
        #endregion
    }

    /// <summary>
    /// Now i have to do something like this to reset all the layers...
    /// </summary>
    public static class GameObjectExtension
    {

        public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
        {
            parent.layer = layer;
            if (includeChildren)
            {
                foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = layer;
                }
            }
        }
    }
}
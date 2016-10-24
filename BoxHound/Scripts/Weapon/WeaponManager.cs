using UnityEngine;
using System.Collections.Generic;
using System;

namespace BoxHound {
    public class WeaponManager : Photon.PunBehaviour
    {
        // All weapon types
        public enum WeaponType
        {
            None,
            WaltherPPK,
            M1911,
            MP40,
            SawedOffShotgun,
        }

        #region Public variables
        // -------------- Public variable -------------
        // The index of weapons in the weapon list for fast look up.
        public int CurrentUsingWeaponIndex
        { get; set; }

        // The reference of the current using weapon.
        public Weapon CurrentUsingWeapon {
            get; private set;
        }

        public int WeaponListCount {
            get { return m_WeaponList.Count; }
        }
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        [Header("Weapons holders")]
        [Tooltip("The transform location of where the weapon will be holded.")]
        [SerializeField]
        // The transform location of where the weapon will be holded.
        private Transform WeaponHolderOnRight;

        private WeaponType defualtWeaponType = WeaponType.WaltherPPK;

        // The list of all available weapons in the game.
        private List<Weapon> m_WeaponList    
            = new List<Weapon>();

        // When viewing leaderboard or in game menu, the weapon is non-usable at those moment.
        private bool m_IsWeaponUsable = false;

        // The pre-set weapon layer, the second camera need this layer to 
        // identify which part of object needs to be renderer differently.
        private readonly string m_WeaponLayer = "Weapon";
        #endregion

        #region Private methods.
        public void Init() {
            // Before finishing the initiation, disable all weapon usage.
            m_IsWeaponUsable = false;

#if UNITY_EDITOR
            #region Safety check for both weapon holder.
            // Check if both weapon holder are ready.
            if (!WeaponHolderOnRight)
                Debug.LogError("Something is wrong with the weapon holder.");
            #endregion
#endif

            #region Get all the available guns form weapon holdier.
            // Clear the weapon list for initializtion.
            m_WeaponList.Clear();

            // Get all weapons under this holder.
            foreach (var weapon in WeaponHolderOnRight.GetComponentsInChildren<Weapon>())
            {
                // Check if any doubled weapon.
                if (!m_WeaponList.Contains(weapon))
                {
                    // Initialize the weapon.
                    // weapon.Init(defualtWeaponType == weapon.GetWeaponType ? true : false);
                    weapon.Init(false);
                    // Add it into the weapon list.
                    m_WeaponList.Add(weapon);
                    // If this character is controll by the client, set weapon layer 
                    // so it can renderer correctly on the camera.
                    if (photonView.isMine) {
                        weapon.gameObject.SetLayer(LayerMask.NameToLayer(m_WeaponLayer));
                    }
                }
            }
#if UNITY_EDITOR
            if (m_WeaponList.Count == 0)
                Debug.LogError("Couldn't find any weapon under the character's weapon holding point.");
#endif
            #endregion

            // Set the first weapon in the list as the starter weapon.
            // TODO: This is going to change by adding the loadout function.

            // Random guns!
            SetWeapon(0);
        }

        public void SetWeapon(int index) {
            CurrentUsingWeaponIndex = index;
            m_WeaponList[CurrentUsingWeaponIndex].Reset();
        }

        private void Update() {
            // If the weapon is non-usable or this object is not controlled by local player,
            // don't do anything.
            if (!photonView.isMine || !m_IsWeaponUsable) return;

            m_WeaponList[CurrentUsingWeaponIndex].Process();
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Enable or disable the usage of the weapon.
        /// </summary>
        /// <param name="enable">Ture if enable.</param>
        public void EnableWeapon(bool enable) {
            m_IsWeaponUsable = enable;
            // If the weapon is going to be disable, force it into hiding state.
            if (!enable)
                m_WeaponList[CurrentUsingWeaponIndex].ForcePullBackWeapon();
        }

        /// <summary>
        /// Switch weapon by the index of the weapon in list.
        /// </summary>
        /// <param name="weaponIndex"></param>
        public void SwitchWeapon(int weaponIndex) {  }

        /// <summary>
        /// Get weapon name by its index in the list.
        /// Since every player all instantiate form a same prefab object, this weapon list should be same. 
        /// </summary>
        /// <param name="index">The index of the weapon on the list.</param>
        /// <returns></returns>
        public string GetWeaponNameByIndex(int index) {
            Weapon weapon = GetWeaponByIndex(index);
            if (weapon) return weapon.GetWeaponName;
            return "some sort of weapon";
        }

        /// <summary>
        /// Get weapon reference by its index in the list.
        /// </summary>
        /// <param name="index">The index of the weapon on the list.</param>
        /// <returns></returns>
        public Weapon GetWeaponByIndex(int index)
        {
            if (index >= 0 && index < m_WeaponList.Count)
            {
                return m_WeaponList[index];
            }
            return null;
        }

        /// <summary>
        /// When player respawn, reset the weapon data.
        /// </summary>
        public void ResetAllWeaponState() {
            // If this character is not control by the local client,
            // no need to touch the data.
            if (!photonView.isMine) return;
            foreach (var weapon in m_WeaponList)
            {
                weapon.Reset();
            }
        }

        public void AddAmmoToCurrentWeapon() {
            m_WeaponList[CurrentUsingWeaponIndex].RefillAmmo();
        }
        #endregion
    }
}
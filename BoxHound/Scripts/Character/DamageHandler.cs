using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BoxHound
{
    /// <summary>
    /// This class handles all the damage changes form outside of the character.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class DamageHandler : Photon.PunBehaviour
    {
        /// <summary>
        /// A warper of the RPC call on take damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int weaponIndex, Transform damageSource, Vector3 hitPosition, int damage) {
            // The poping damage number is happen in loacl.
            // CharacterManager.LocalPlayer.GetPlayerHUD.PopFloatingNumber(hitPosition, damage);
            // Send the damage info to all the remote object.
            photonView.RPC("OnSyncDamage", photonView.owner, weaponIndex, damage, hitPosition);
        }

        [PunRPC]
        private void OnSyncDamage(int weaponIndex, int damage, Vector3 attackerPosition, PhotonMessageInfo attackerinfo)
        {
            // If the health points reach ZERO, this character has been eliminated
            // so it can't recevie more damage until it respawn.
            if (CharacterManager.LocalPlayer.CurrentHealth == 0) return;

            #region Damage indicator
            DamageIndicatorInfo info = new DamageIndicatorInfo(attackerPosition);
            info.AttackerID = attackerinfo.photonView.ownerId;
            DamageIndicatorManager.OnDamageIndicator(info);
            #endregion

            #region Health points
            // Deal damage to the character.
            CharacterManager.LocalPlayer.DealDamage(damage, attackerinfo, weaponIndex);
            #endregion
        }
    }
}

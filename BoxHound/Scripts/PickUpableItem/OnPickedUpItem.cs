using UnityEngine;
using System.Collections;

namespace BoxHound.Larry
{
    public class OnPickedUpItem : MonoBehaviour
    {
        public enum PickUpableType
        {
            Ammo,
            Shotgun,
            SMG,
        }

        public PickUpableType pickupableType = PickUpableType.Ammo;

        public void OnPickedUp(PickupItem item)
        {
            if (item.PickupIsMine)
            {
                switch (pickupableType) {
                    case PickUpableType.Ammo:
                        CharacterManager.LocalPlayer.GetWeaponManager.AddAmmoToCurrentWeapon();
                        break;
                    case PickUpableType.Shotgun:
                        break;
                    case PickUpableType.SMG:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;
namespace Larry.BoxHound
{
    public abstract class Weapon : MonoBehaviour
    {
        // All weapon types
        public enum Type {
            None,
            Handgun,
            RevolverLong,
            RevolverShort,
            Sniper,
            BoltActionSniper,
            SubmachineGun,
            AssaultRifle,
            Shotgun,
            SawnOffShotGun,
        }

        #region Public classes
        [System.Serializable]
        protected class WeaponDataDetail {
            [Header("Customizable weapon data")]
            // The weapon type.
            // If we are using the .net 4.0 or greater, with the new version 6 C#:
            // public Type WeaponType { get; private set; } = Type.none;
            // But Unity3D still under .net 2.0.
            public Type WeaponType = Type.None;
            // The name of the weapon, such as "Thomson", "Sawn Off Shotgun".
            public string WeaponName = "";
            // How many shots contains in one magazine/clip.
            public int MagazineSize = 0;
            // How many bullets or how much energy there are left.
            public int BulletsLeft = 0;
            // How fast the weapon can shoot or use (Not used for all weapons).
            public float FireRate = 0;
            // The duration of reloading the weapon.
            public float ReloadDuration = 1.5f;
            // How far the bullet raycast will reach, default value is 150.0f.
            public float BulletDistance = 150.0f;
            // How "Hard" the bullet will hit on rigidBodies, default value is 15.0f.
            public float BulletForce = 15f;
        }

        [System.Serializable]
        protected class BulletImpacts
        {
            [Header("Bullet impacts and Tags")]
            [Header("Metal")]
            // The prefab of bullet impacts, 
            // spawn when bullet hits on a non-movable scene object taged as "Metal".
            public Transform MetalImpactStaticPrefab;
            // Same as above, but this perfab is for movable objects, 
            // the bullet hole sprite will spwan by the hitted object.
            public Transform MetalImpactPrefab;
            [Header("Wood")]
            // The prefab of bullet impacts,
            // spawn when bullet hits on a non-movable scene object taged as "Wood".
            public Transform WoodImpactStaticPrefab;
            // The prefab of bullet impacts spawn on movable object taged as "Wood".
            public Transform WoodImpactPrefab;
            [Header("Concrete")]
            // The prefab of bullet impacts,
            // spawn when bullet hits on a non-movable scene object taged as "Concrete".
            public Transform ConcreteImpactStaticPrefab;
            // The prefab of bullet impacts spawn on movable object taged as "Concrete".
            public Transform ConcreteImpactPrefab;
            [Header("Dirt")]
            // The prefab of bullet impacts,
            // spawn when bullet hits on a non-movable scene object taged as "Dirt".
            public Transform DirtImpactStaticPrefab;
            // The prefab of bullet impacts spawn on movable object taged as "Dirt".
            public Transform DirtImpactPrefab;

            [Header("Impact Tags")]
            // Default tags for bullet impacts.
            public static readonly string MetalImpactStaticTag = "Metal (Static)";
            public static readonly string MetalImpactTag = "Metal";
            public static readonly string WoodImpactStaticTag = "Wood (Static)";
            public static readonly string WoodImpactTag = "Wood";
            public static readonly string ConcreteImpactStaticTag = "Concrete (Static)";
            public static readonly string ConcreteImpactTag = "Concrete";
            public static readonly string DirtImpactStaticTag = "Dirt (Static)";
            public static readonly string DirtImpactTag = "Dirt";
        }

        [System.Serializable]
        protected class MuzzleFlashes
        {
            [Header("Muzzleflash Holders")]
            // All components of the muzzle flash.
            public SpriteRenderer SideViewMuzzle;
            public SpriteRenderer TopViewMuzzle;
            public SpriteRenderer FrontViewMuzzle;
            [Tooltip("The light source for simulating the muzzle flash.")]
            public Light MuzzleFlashLight;
            [Tooltip("Array of muzzleflash sprites, switch between these sprites when firing.")]
            public Sprite[] MuzzleflashSideSprites;
            // How long the muzzle flash can be visible, default value is 0.02f.
            public float MuzzleFlashDuration = 0.02f;
            // How intense the muzzle flash will be, default value is 2.0f.
            [Range(1f, 4f)]
            public float MuzzleFlashLightIntensity = 2.0f;
            // The range of the muzzle flash lighting, default value is 10.0f.
            [Range(5f, 50f)]
            public float MuzzleFlashLigthRange = 10.0f;

            public void ShowSideMuzzleFlashSprite(bool show) {
                SideViewMuzzle.enabled = show;
                TopViewMuzzle.enabled = show;
            }

            public void ShowFrontMuzzleFlashSprite(bool show)
            {
                FrontViewMuzzle.enabled = show;
            }

            public void ShowMuzzleFlashLight(bool show) {
                MuzzleFlashLight.enabled = show;
            }
        }

        [System.Serializable]
        protected class SpawnPoints
        {
            [Tooltip("The spawn point of the ammo case durling firing.")]
            [Header("SpawnPoints and Prefabs")]
            public Transform CasingSpawnPoint;
            [Tooltip("The weapon's ammo case prefab.")]
            public Transform CasingPrefab;
            [Tooltip("The empty magazine/Clip prefab")]
            public Transform EmptyMagPrefab;
            [Tooltip("The raycast will start at the bullet spawnpoint, going forward")]
            public Transform BulletSpawnPoint;
            [Tooltip("The new mag will spawn at this poistion when reloading")]
            public Transform MagSpawnPoint;
        }
        #endregion

        #region Protected variables
        [SerializeField]
        protected WeaponDataDetail m_WeaponData;
        [SerializeField]
        protected BulletImpacts m_ImpactsAndTags;
        [SerializeField]
        protected MuzzleFlashes m_MuzzleFlash;
        [SerializeField]
        protected SpawnPoints m_SpawnPoint;

        // Used to prevent gun switching while reloading and shooting.
        protected bool m_CanSwitchWeapon = false;
        // Return TRUE if the weapon is out of ammo or energy.
        protected bool m_OutOfAmmo = false;
        // Return TRUE if the weapon currenly is reloading ammo or energy.
        protected bool m_IsReloading = false;
        // Record the last fired time used for calculate fire rate.
        protected float m_LastFiredTimeStamp = 0.0f;

        protected PhotonView m_view;
        #endregion

        #region Public methods.
        public virtual void Init(PhotonView managerView) {

            m_view = managerView;
            //Set the magazine size
            m_WeaponData.BulletsLeft = m_WeaponData.MagazineSize;

            // A temporary way to update the UI
            GameUIManager.Instance.GameInfoUI.UpdateAmmoCountUI(false, 
                m_WeaponData.WeaponName, 
                m_WeaponData.BulletsLeft, 
                m_WeaponData.MagazineSize);

            #region Muzzle flash light and sprite.
            // Turn off the muzzle flash light at beginning.
            m_MuzzleFlash.MuzzleFlashLight.enabled = false;
            // Initialize light setting.
            m_MuzzleFlash.MuzzleFlashLight.intensity = m_MuzzleFlash.MuzzleFlashLightIntensity;
            m_MuzzleFlash.MuzzleFlashLight.range = m_MuzzleFlash.MuzzleFlashLigthRange;

            m_MuzzleFlash.ShowSideMuzzleFlashSprite(false);
            m_MuzzleFlash.ShowFrontMuzzleFlashSprite(false);
            #endregion

            // Hide the weapon at the beginning of the game.
            ShowWeapon(false);
        }

        public virtual void ShowWeapon(bool show) {
            this.gameObject.SetActive(!show);
        }

        public abstract void Process();
        #endregion

        #region Private methods.
        protected IEnumerator MuzzleFlash()
        {
            // Raycast bullet
            RaycastHit hit;
            // Shoot a ray directly form the gun model.
            Ray ray = new Ray(transform.position, transform.forward);

            #region Raycast bullet from bullet spawn point.
            //Send out the raycast from the "bulletSpawnPoint" position
            if (Physics.Raycast(m_SpawnPoint.BulletSpawnPoint.position,
                m_SpawnPoint.BulletSpawnPoint.forward, out hit))
            {

                // If a rigibody is hit, add bullet force to it.
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(ray.direction * m_WeaponData.BulletForce);
                }

                // If the raycast hit the tag "Target"
                if (hit.transform.tag == "Target")
                {
                    Instantiate(m_ImpactsAndTags.MetalImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                BulletHitOnObject(hit);
            }
            #endregion

            #region Show the muzzle flash            
            if (true) {
                // Chooses a random muzzleflash sprite from the array
                m_MuzzleFlash.SideViewMuzzle.sprite =
                    m_MuzzleFlash.MuzzleflashSideSprites
                    [Random.Range(0, m_MuzzleFlash.MuzzleflashSideSprites.Length)];
                m_MuzzleFlash.SideViewMuzzle.sprite =
                    m_MuzzleFlash.MuzzleflashSideSprites
                    [Random.Range(0, m_MuzzleFlash.MuzzleflashSideSprites.Length)];

                m_MuzzleFlash.ShowSideMuzzleFlashSprite(true);
                m_MuzzleFlash.ShowFrontMuzzleFlashSprite(true);
            }
            #endregion

            #region Show the muzzle flash light
            // Turn on muzzle flash light
            m_MuzzleFlash.ShowMuzzleFlashLight(true);
            // Wait for set amount of time, default value 0.02
            yield return new WaitForSeconds(m_MuzzleFlash.MuzzleFlashDuration);
            // Hide the muzzle flashes
            m_MuzzleFlash.ShowSideMuzzleFlashSprite(false);
            m_MuzzleFlash.ShowFrontMuzzleFlashSprite(false);
            // Turn off muzzle flash light
            m_MuzzleFlash.ShowMuzzleFlashLight(false);
            #endregion
        }

        private void BulletHitOnObject(RaycastHit hit) {
            #region Hit on metal
            // If the raycast hit the tag "Metal (Static)"
            if (hit.transform.tag == BulletImpacts.MetalImpactStaticTag)
            {
                // Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.MetalImpactStaticPrefab, hit.point,
                    Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Metal"
            if (hit.transform.tag == BulletImpacts.MetalImpactTag)
            {
                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.MetalImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion

            #region Hit on wood
            //If the raycast hit the tag "Wood (Static)"
            if (hit.transform.tag == BulletImpacts.WoodImpactStaticTag)
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.WoodImpactStaticPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Wood"
            if (hit.transform.tag == BulletImpacts.WoodImpactTag)
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.WoodImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion

            #region Hit on Concrete
            //If the raycast hit the tag "Concrete (Static)"
            if (hit.transform.tag == BulletImpacts.ConcreteImpactStaticTag)
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.ConcreteImpactStaticPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Concrete"
            if (hit.transform.tag == BulletImpacts.ConcreteImpactTag)
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.ConcreteImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion

            #region Hit on Dirt
            //If the raycast hit the tag "Dirt (Static)"
            if (hit.transform.tag == BulletImpacts.DirtImpactStaticTag)
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.DirtImpactStaticPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Dirt"
            if (hit.transform.tag == BulletImpacts.DirtImpactTag)
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.DirtImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion
        }

        #endregion
    }
}

using UnityEngine;
using System.Collections;
using BoxHound.UI;

namespace BoxHound
{
    public abstract class Weapon : Photon.PunBehaviour
    {
        public enum WeaponHoldingPhase
        {
            PullingOut,
            Holding,
            PullingBack,
            Hiding
        }

        public enum AmmoState
        {
            Normal,
            Reloading,
            ClipOutOfAmmo,
            OutOfAmmo,
        }

        #region Public classes
        [System.Serializable]
        protected class WeaponDataDetail {
            [Header("Customizable weapon data")]
            // The weapon type.
            // If we are using the .net 4.0 or greater, with the new version 6 C#:
            // public Type WeaponType { get; private set; } = Type.none;
            // But Unity3D still under .net 2.0.
            public WeaponManager.WeaponType WeaponType = WeaponManager.WeaponType.None;
            // The name of the weapon, such as "Thomson", "Sawn Off Shotgun".
            public string WeaponName = "";
            // The effect range of the weapon.
            public float EffectRange = 50.0f;
            // How many shots contains in one magazine/clip.
            public int MagazineSize = 0;
            // How many magazine can this weapon hold.
            public int MagazineCount = 0;
            // How fast the weapon can shoot or use (Not used for all weapons).
            public float FireRate = 0;
            // The duration of Pulling the weapon in and out.
            public float PullDuration = 0.05f;
            // The duration of reloading the weapon.
            public float ReloadDuration = 1.5f;
            // How far the bullet raycast will reach, default value is 150.0f.
            public float BulletDistance = 150.0f;
            // How "Hard" the bullet will hit on rigidBodies, default value is 15.0f.
            public float BulletForce = 15f;
            // How much damage will do when hits on target.
            public int DamagePerShot = 12;
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
                // Some weapon don't have front view muzzle flash.
                if (FrontViewMuzzle == null) return;
                FrontViewMuzzle.enabled = show;
            }

            public void ShowMuzzleFlashLight(bool show) {
                MuzzleFlashLight.enabled = show;
            }
        }
        #endregion

        #region Public variables
        // -------------- Public variable -------------
        /// <summary>
        /// Get the current weapon holding phase.
        /// </summary>
        public WeaponHoldingPhase GetWeaponHoldingPhase {
            get { return m_Phase; }
        }
        #endregion

        #region Protected variables
        // -------------- Protected variable -------------
        [SerializeField]
        protected WeaponDataDetail m_WeaponData;
        [SerializeField]
        protected BulletImpacts m_ImpactsAndTags;
        [SerializeField]
        protected MuzzleFlashes m_MuzzleFlash;

        [SerializeField]
        protected AudioSource m_PicKedUpItemAudioSource;
        [SerializeField]
        protected AudioClip m_FillUpAmmoSound;

        // Used to prevent gun switching while reloading and shooting.
        protected bool m_CanSwitchWeapon = false;
        // Return TRUE if the weapon is out of ammo or energy.
        protected bool m_OutOfAmmo = false;
        // Return TRUE if the weapon currenly is reloading ammo or energy.
        protected bool m_IsReloading = false;
        // Record the last fired time used for calculate fire rate.
        protected float m_LastFiredTimeStamp = 0.0f;
        // Is this the default weapon? Default weapon has unlimit ammo and can't be drop.
        protected bool m_DefaultWeapon = false;
        // How many bullets or energy have left in clip.
        protected int m_BulletsLeftInClip = 0;
        // How many bullets can this weapon hold.
        protected int m_RestBulletsCount = 0;
        // How many magazines have left.
        protected int m_MagazineLeft = 0;
        // Is there has one round remain in the chamber when reload?
        protected bool m_OneRoundRemainInChamber = false;

        // The corss hair.
        protected CorssHairUI m_CorssHair;
        // The Weapon info
        protected WeaponInfoUI m_WeaponInfo;

        // The layermask for the bullet raycasting.
        protected LayerMask m_BulletLayerMask;
        // The target gets hit.
        CharacterManager m_TargetCharacter;


        // The current weapon holding phase.
        protected WeaponHoldingPhase m_Phase = WeaponHoldingPhase.Hiding;
        #endregion

        #region Public methods.
        public virtual void Init(bool isDefaultWeapon) {
            // Ignore the object taged as "LocalPlayer" so we won't shot ourself by accident.
            // Also ignore the object taged as "Respawn" because those respawn points has it's own collider for
            // detecting if any obstacle in the area.
            m_BulletLayerMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Ignore Raycast"));
            // Is this the default weapon?
            m_DefaultWeapon = isDefaultWeapon;
            // Used to prevent gun switching while reloading and shooting.
            m_CanSwitchWeapon = false;
            // Return TRUE if the weapon is out of ammo or energy.
            m_OutOfAmmo = false;
            // Return TRUE if the weapon currenly is reloading ammo or energy.
            m_IsReloading = false;
            // Record the last fired time used for calculate fire rate.
            m_LastFiredTimeStamp = 0.0f;
            // The weapon will remian hiding until player pull it out.
            m_Phase = WeaponHoldingPhase.Hiding;
            // Fill the cilp.
            m_BulletsLeftInClip = m_WeaponData.MagazineSize;
            // The magazines currently holding.
            m_MagazineLeft = m_WeaponData.MagazineCount;
            // Grab all the ammo as you can.
            m_RestBulletsCount = m_WeaponData.MagazineSize * m_MagazineLeft;
            // The gun is unsed, so the chamber is clean.
            m_OneRoundRemainInChamber = false;

            // Only the character which is control by the local client needs update the UI.
            if (photonView.isMine) {
                // The corss hair.
                m_CorssHair = UIframework.UIManager.Instance.GetUI<PlayerHUDUI>(UIframework.UIManager.SceneUIs.PlayerHUDUI).GetCorssHair;

                m_WeaponInfo = UIframework.UIManager.Instance.GetUI<PlayerHUDUI>(UIframework.UIManager.SceneUIs.PlayerHUDUI).GetWeaponInfo;

                if (isDefaultWeapon) {
                    // Update the weapon UI



                    m_WeaponInfo.UpdateWeaponInfo(AmmoState.Normal,
                        m_WeaponData.WeaponName,
                        m_BulletsLeftInClip,
                        m_RestBulletsCount,
                        m_WeaponData.MagazineSize,
                        m_DefaultWeapon);
                }
            }

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
            DisplayWeaponModel(false);
        }

        // This runs in the weapon manager.
        public abstract void Process();

        /// <summary>
        /// Get weapon name.
        /// </summary>
        public string GetWeaponName {
            get { return m_WeaponData.WeaponName; }
        }

        /// <summary>
        /// Get weapon type.
        /// </summary>
        public WeaponManager.WeaponType GetWeaponType {
            get { return m_WeaponData.WeaponType; }
        }

        /// <summary>
        /// Reset weapon data such as ammo count and damage value to default.
        /// </summary>
        public abstract void Reset();

        public void RefillAmmo() {
            m_PicKedUpItemAudioSource.PlayOneShot(m_FillUpAmmoSound);

            if (m_DefaultWeapon) return;
            m_RestBulletsCount = m_WeaponData.MagazineCount * m_WeaponData.MagazineSize;
            m_BulletsLeftInClip = m_WeaponData.MagazineSize;
            // Update the weapon UI
            GameRoomUI.Instance.PlayerHUDUI.UpdateAmmoCountUI(PlayerHUD.AmmoState.Normal,
                m_WeaponData.WeaponName,
                m_BulletsLeftInClip,
                m_RestBulletsCount,
                m_WeaponData.MagazineSize,
                m_DefaultWeapon);
        }

        /// <summary>
        /// A function for force pulling back the weapon. 
        /// Usefull in cases such as opening the game menu or opening the team select menu.
        /// </summary>
        public abstract void ForcePullBackWeapon();
        #endregion

        #region Private methods.
        /// <summary>
        /// The muzzle flash animation, 
        /// including the muzzle flash sprite and flash lighting.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator MuzzleFlash()
        {
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

        /// <summary>
        /// Cast a ray form the gun as the bullet and detecte if it hits anything. 
        /// </summary>
        protected void RayCastSingleBullet(Transform origin)
        {
            // Raycast bullet
            RaycastHit hit;
            // Shoot a ray directly form the gun model.
            // Ray ray = new Ray(transform.position, transform.forward);

            #region Raycast bullet from bullet spawn point.
            //Send out the raycast from the "bulletSpawnPoint" position
            if (Physics.Raycast(origin.position, origin.forward, out hit, m_WeaponData.BulletDistance, m_BulletLayerMask))
            {
                // If the raycast hit gameObject the taged as "Target"
                if (hit.transform.tag == "Target")
                {
                    m_TargetCharacter = hit.transform.GetComponent<CharacterManager>();

                    if (RoomManager.CurrentGameMode.GetGameMode() == GameModeManager.GameModes.FreeForAll)
                    {
                        m_TargetCharacter.HandleDamage.TakeDamage(
                        CharacterManager.LocalPlayer.GetWeaponManager.CurrentUsingWeaponIndex,
                        origin,
                        hit.point,
                        // If the hit distance between the attacker and the target is out of the effect range, damage reduce by half.
                        // ((hit.distance <= m_WeaponData.EffectRange) ? m_WeaponData.DamagePerShot : Mathf.RoundToInt(m_WeaponData.DamagePerShot / 2)));
                        m_WeaponData.DamagePerShot);
                    }
                    else
                    {
                        if (m_TargetCharacter.Team != CharacterManager.LocalPlayer.Team)
                        {
                            m_TargetCharacter.HandleDamage.TakeDamage(
                            CharacterManager.LocalPlayer.GetWeaponManager.CurrentUsingWeaponIndex,
                            origin,
                            hit.point,
                            // If the hit distance between the attacker and the target is out of the effect range, damage reduce by half.
                            // ((hit.distance <= m_WeaponData.EffectRange) ? m_WeaponData.DamagePerShot : Mathf.RoundToInt(m_WeaponData.DamagePerShot / 2)));
                            m_WeaponData.DamagePerShot);
                        }
                    }

                    // Spawn bullet impact on surface
                    Instantiate(m_ImpactsAndTags.WoodImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                // Visualize the hit result by detecting what kinds of target did the bullet hits.
                BulletStrikePoint(hit);
            }
            #endregion
        }

        protected void AutomaticTypeWeaponReload() {
            // If this weapon is the default weapon.
            if (m_DefaultWeapon) {
                if (m_BulletsLeftInClip != 0)
                {
                    m_BulletsLeftInClip = m_WeaponData.MagazineSize + 1;
                    m_OneRoundRemainInChamber = true;
                }
                else {
                    m_BulletsLeftInClip = m_WeaponData.MagazineSize;
                    m_OneRoundRemainInChamber = false;
                }
                return;
            }

            // Check how many rounds has been fired and reduce the number form the rest of the bullets.
            int firedRounds = m_WeaponData.MagazineSize + (m_OneRoundRemainInChamber ? 1 : 0) - m_BulletsLeftInClip;

            if (firedRounds > m_RestBulletsCount)
            {
                m_BulletsLeftInClip += m_RestBulletsCount;
                m_RestBulletsCount = 0;
            }
            else
            {
                m_RestBulletsCount -= firedRounds;
                if (m_RestBulletsCount <= 0) m_RestBulletsCount = 0;

                // If rest of the bullets can't even fill a magazine (But there still has bullets in the gun).
                if (m_RestBulletsCount < m_WeaponData.MagazineSize)
                {
                    // Reload with what have left.
                    m_BulletsLeftInClip += m_RestBulletsCount;
                    m_OneRoundRemainInChamber = false;

                    // If still can fill up a magazine, that means there has one round in the chamber.
                    if (m_BulletsLeftInClip >= m_WeaponData.MagazineSize + 1)
                    {
                        // Resize the amount of the bullet.
                        m_BulletsLeftInClip = m_WeaponData.MagazineSize + 1;
                        m_OneRoundRemainInChamber = true;
                    }
                }
                // Else of the rest of the bullets enough fill a magazine.
                else
                {
                    // If reload when the weapon is not completely dry
                    if (m_BulletsLeftInClip != 0)
                    {
                        // That will remian one round in the chamber
                        m_BulletsLeftInClip = m_WeaponData.MagazineSize + 1;
                        m_OneRoundRemainInChamber = true;
                    }
                    // Else if reload when the weapon is competely dry.
                    else
                    {
                        // Insert a new magazine.
                        m_BulletsLeftInClip = m_WeaponData.MagazineSize;
                        m_OneRoundRemainInChamber = false;
                    }
                }
            }
        }

        protected void SawedOffShotgunReload() {
            int firedRounds = m_WeaponData.MagazineSize - m_BulletsLeftInClip;

            // If this weapon is the default weapon.
            if (m_DefaultWeapon)
            {
                m_BulletsLeftInClip = m_WeaponData.MagazineSize;
                return;
            }

            if (firedRounds >= m_RestBulletsCount)
            {
                m_BulletsLeftInClip += m_RestBulletsCount;
                m_RestBulletsCount = 0;
            }
            else
            {
                m_RestBulletsCount -= firedRounds;
                if (m_RestBulletsCount <= 0) m_RestBulletsCount = 0;

                // If rest of the bullets can't even fill a magazine (But there still has bullets in the gun).
                if (m_RestBulletsCount < m_WeaponData.MagazineSize)
                {
                    // Reload with what have left.
                    m_BulletsLeftInClip += m_RestBulletsCount;
                }
                else
                {
                    // Insert a new magazine.
                    m_BulletsLeftInClip = m_WeaponData.MagazineSize;
                }
            }
        }

        /// <summary>
        /// Controll the weapon model renderer.
        /// </summary>
        /// <param name="show">Set TRUE if display the weapon.</param>
        public abstract void DisplayWeaponModel(bool show);

        /// <summary>
        /// When the bullet hits on a surface, 
        /// show same particle effect and sound effect.
        /// </summary>
        /// <param name="hit">Where the bullet hits.</param>
        protected void BulletStrikePoint(RaycastHit hit) {
            #region Hit on metal
            // If the raycast hit the tag "Metal (Static)"
            if (hit.transform.CompareTag(BulletImpacts.MetalImpactStaticTag))
            {
                // Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.MetalImpactStaticPrefab, hit.point,
                    Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Metal"
            if (hit.transform.CompareTag(BulletImpacts.MetalImpactTag))
            {
                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.MetalImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion

            #region Hit on wood
            //If the raycast hit the tag "Wood (Static)"
            if (hit.transform.CompareTag(BulletImpacts.WoodImpactStaticTag))
            {
                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.WoodImpactStaticPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Wood"
            if (hit.transform.CompareTag(BulletImpacts.WoodImpactTag))
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.WoodImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion

            #region Hit on Concrete
            //If the raycast hit the tag "Concrete (Static)"
            if (hit.transform.CompareTag(BulletImpacts.ConcreteImpactStaticTag))
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.ConcreteImpactStaticPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Concrete"
            if (hit.transform.CompareTag(BulletImpacts.ConcreteImpactTag))
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.ConcreteImpactPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }
            #endregion

            #region Hit on Dirt
            //If the raycast hit the tag "Dirt (Static)"
            if (hit.transform.CompareTag(BulletImpacts.DirtImpactStaticTag))
            {

                //Spawn bullet impact on surface
                Instantiate(m_ImpactsAndTags.DirtImpactStaticPrefab, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                return;
            }

            //If the raycast hit the tag "Dirt"
            if (hit.transform.CompareTag(BulletImpacts.DirtImpactTag))
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

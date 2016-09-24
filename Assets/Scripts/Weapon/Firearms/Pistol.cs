using UnityEngine;
using System.Collections;
using System;

namespace Larry.BoxHound
{
    public class Pistol : Weapon
    {
        #region Public classes
        [System.Serializable]
        private class WeaponHolders {
            [Header("Handgun Holders")]
            [Tooltip("The root of the weapon object.")]
            public GameObject Holder;
            [Tooltip("The handgun's slide.")]
            public GameObject Slide;
            [Tooltip("The magazine that is alread inserted in the weapon, " +
                "when reload this will become invisible untill the reload animation finished.")]
            public MeshRenderer InGunMagazine;
            [Tooltip("Used in reloading animating and only visible durling reload.")]
            public GameObject NewFullMagzine;

            [Header("Particle System")]
            // Particle system
            public ParticleSystem SmokeParticles;
        }
        [System.Serializable]
        private class Animations {
            [Header("Pistol Animation compoment")]
            [Tooltip("The animation compoment on the pistol's mainframe.")]
            public Animation WeaponMainFrame;
            [Tooltip("The animation compoment on the pistol's slide.")]
            public Animation WeaponSlide;
            [Tooltip("The animation compoment on the pistol's new magazine for reload.")]
            public Animation NewMagazine;

            [Header("Pistol Animation clip name")]
            // Handgun animations
            [Tooltip("The recoil animtion when firing the weapon.")]
            public string Recoil;
            [Tooltip("The preparation of reloading animtion, " +
                "does not include mag inserting or single round loading animation," +
                " just rise the weapon and prepar for reload.")]
            public string Reload;
            [Tooltip("Slide move back and forwad after shooting and load next round if it has any.")]
            public string SlideReload;
            [Tooltip("Insert a full magazine into the weapon.")]
            public string FullMagInsert;
            [Tooltip("Blow back the slide when fired each round.")]
            public string SlideBlowback;
            [Tooltip("Slide snap forward after loaded a new magazine, cambering a round from the loaded magazine.")]
            public string SlideCocking;
            [Tooltip("When the gun is out of ammo, slide lock back after the last shot.")]
            public string SlideLockBack;
        }
        [System.Serializable]
        private class WeaponSounds {
            [Tooltip("Shooting Sounds")]
            [Header("Weapon Sound source")]
            public AudioSource Firing = null;
            public AudioSource Sliding = null; 
            public AudioSource Reloading = null;
            public AudioSource Magazine = null;

            [Header("Firing Sounds")]
            [Tooltip("Main fire sound")]
            public AudioClip SigleShotSound;
            [Tooltip("The sound of clicking the trigger while out of ammo.")]
            public AudioClip DryFireSound;

            [Header("Sliding Sounds")]
            [Tooltip("Slide reload sound after weapon fully empty.")]
            public AudioClip SlideReloadSound;
            [Tooltip("Slide snap forward sound after loaded a new round.")]
            public AudioClip SlideSnap;

            [Header("Reloading Sounds")]
            [Tooltip("The sound of removing a magzine.")]
            public AudioClip RemoveMag;
            [Tooltip("The sound of inserting a magzine.")]
            public AudioClip InsertMag;
        }
        #endregion

        #region Public variables.
        [SerializeField]
        private WeaponHolders m_WeaponHolder;
        [SerializeField]
        private Animations m_Animation;
        [SerializeField]
        private WeaponSounds m_WeaponSound;

        //For the handgun animation
        private bool m_HasPlayedOutOfAmmoAnimation = false;
        private bool m_SliderLockbacked = false;
        #endregion

        public override void Init(PhotonView managerView)
        {
            base.Init(managerView);
        }

        public override void Process()
        {
            #region Firing
            // If the user just click right mouse button
            // while the gun is not out of ammo and its not durling reloading.
            if (Input.GetMouseButtonDown(0) && !m_OutOfAmmo && !m_IsReloading)
            {
                // Shot Animation sync through the network
                photonView.RPC("WeaponFireAnimation", PhotonTargets.All);

                // Cast a ray as the bullet.
                photonView.RPC("RayCastBullet", PhotonTargets.All, 
                    m_SpawnPoint.BulletSpawnPoint.position, 
                    m_SpawnPoint.BulletSpawnPoint.forward);

                // Reduce one round form the magazine/clip after shooting and
                // update the local client's UI.
                ReduceOneRound();
            }
            #endregion

            #region If out of ammo
            if (m_WeaponData.BulletsLeft == 0 && !m_IsReloading)
            {
                // Play the slide animation once only
                if (!m_HasPlayedOutOfAmmoAnimation)
                {
                    // Out of ammo animation sync through the network
                    photonView.RPC("OutOfAmmoAnimation", PhotonTargets.All);
                }

                // Set flags on local client.
                OnWeaponDry();
            }
            #endregion

            #region Reloading
            if (Input.GetKeyDown(KeyCode.R) &&
                m_WeaponData.BulletsLeft <= m_WeaponData.MagazineSize &&
                !m_IsReloading)
            {
                // The reload animation will sync through the network, else
                // will be only effect on loacl client.
                OnReloading();
            }
            #endregion
        }

        #region When weapon firing
        /// <summary>
        /// Synchronize The muzzle flash and weapon firing animation through the network.
        /// </summary>
        [PunRPC]
        private void WeaponFireAnimation() {
            // Show muzzle flash
            StartCoroutine(MuzzleFlash());

            // Player weapon recoil animation.
            m_Animation.WeaponMainFrame.Play(m_Animation.Recoil);
            // Player weapon slide blowback animation.
            m_Animation.WeaponSlide.Play(m_Animation.SlideBlowback);

            // Play firing sound with a different tones every time it shoots.
            m_WeaponSound.Firing.pitch = UnityEngine.Random.Range(.8f, 1.2f);
            m_WeaponSound.Firing.PlayOneShot(m_WeaponSound.SigleShotSound);

            // Play smoke particles
            m_WeaponHolder.SmokeParticles.Play();

            // Spawn casing
            Instantiate(m_SpawnPoint.CasingPrefab, m_SpawnPoint.CasingSpawnPoint.position,
                            m_SpawnPoint.CasingSpawnPoint.rotation);
        }

        /// <summary>
        /// Cast a ray form the gun as the bullet and detecte if it hits anything. 
        /// </summary>
        [PunRPC]
        public void RayCastBullet(Vector3 startLocation, Vector3 direction)
        {
            // Raycast bullet
            RaycastHit hit;
            // Shoot a ray directly form the gun model.
            // Ray ray = new Ray(transform.position, transform.forward);

            #region Raycast bullet from bullet spawn point.
            //Send out the raycast from the "bulletSpawnPoint" position
            if (Physics.Raycast(startLocation, direction, out hit))
            {

                // If a rigibody is hit, add bullet force to it.
                //if (hit.rigidbody != null)
                //{
                //    hit.rigidbody.AddForce(ray.direction * m_WeaponData.BulletForce);
                //}

                // If the raycast hit gameObject the taged as "Target"
                if (hit.transform.tag == "Target")
                {
                    //Instantiate(m_ImpactsAndTags.MetalImpactPrefab, hit.point,
                    //    Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                // Visualize the hit result by detecting what kinds of target did the bullet hits.
                BulletHitOnObject(hit);
            }
            #endregion
        }

        /// <summary>
        /// After the shot animation, reduce one round on the magazine.
        /// Do the math and update the UI only in local client.
        /// </summary>
        private void ReduceOneRound() {
            // Remove one bullet everytime you shoot
            m_WeaponData.BulletsLeft -= 1;

            // A temporary way to update the UI
            GameUIManager.Instance.GameInfoUI.UpdateAmmoCountUI(false,
                m_WeaponData.WeaponName,
                m_WeaponData.BulletsLeft,
                m_WeaponData.MagazineSize);
        }
        #endregion

        #region When weapon dry
        // Only set flags in local client, 
        // in other player's scene show only the animation is enough.
        private void OnWeaponDry()
        {
            // Update the local client's UI.
            GameUIManager.Instance.GameInfoUI.UpdateAmmoCountUI(false,
                m_WeaponData.WeaponName,
                m_WeaponData.BulletsLeft,
                m_WeaponData.MagazineSize);

            m_OutOfAmmo = true;
            //Play the slide animation once only
            if (!m_HasPlayedOutOfAmmoAnimation)
            {
                // Only play once.
                m_HasPlayedOutOfAmmoAnimation = true;

                // Slider lockbacked
                m_SliderLockbacked = true;
            }

            // When user clicked right mouse button while the weapon is dry and not durling reloading.
            if (Input.GetMouseButtonDown(0) && !m_IsReloading)
            {
                //Play dry fire sound if clicking when out of ammo
                m_WeaponSound.Firing.PlayOneShot(m_WeaponSound.DryFireSound);
            }
        }

        [PunRPC]
        private void OutOfAmmoAnimation() {
            // Play the slide lock back animation
            m_Animation.WeaponSlide.Play(m_Animation.SlideLockBack);

            //Play slider sound
            m_WeaponSound.Sliding.PlayOneShot(m_WeaponSound.SlideSnap);
        }
        #endregion
        
        #region When reloading
        private void OnReloading() {
            StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            // Can't switch weapon while reloading.
            m_CanSwitchWeapon = true;

            // Start weapon reloading sequence.
            m_IsReloading = true;

            // A temporary way to update the UI
            GameUIManager.Instance.GameInfoUI.UpdateAmmoCountUI(m_IsReloading,
            m_WeaponData.WeaponName,
            m_WeaponData.BulletsLeft,
            m_WeaponData.MagazineSize);

            // Spawn a empty magazine and it will drop down by itseft.
            //Instantiate(m_SpawnPoint.EmptyMagPrefab, m_SpawnPoint.MagSpawnPoint.position,
            //    m_SpawnPoint.MagSpawnPoint.rotation);

            // Hide the new instantiate empty magazine.
            m_WeaponHolder.InGunMagazine.enabled = false;

            // Reloading Animation sync through the network
            photonView.RPC("ReloadingAnimation", PhotonTargets.All);

            // Wait for set amount of time
            yield return new WaitForSeconds(1.2f);

            // Play remove mag sound
            m_WeaponSound.Magazine.PlayOneShot(m_WeaponSound.InsertMag);

            // Wait for set amount of time
            yield return new WaitForSeconds(.4f);

            // Refill bullets, if reload when the gun is not fully empty,
            // one round will stay in the chamber.
            if (m_WeaponData.BulletsLeft != 0)
                m_WeaponData.BulletsLeft = m_WeaponData.MagazineSize + 1;
            else
                m_WeaponData.BulletsLeft = m_WeaponData.MagazineSize;

            // Make the magazine visible again.
            m_WeaponHolder.InGunMagazine.enabled = true;

            // Reload complete, weapon is hot.
            m_HasPlayedOutOfAmmoAnimation = false;
            m_OutOfAmmo = false;
            m_IsReloading = false;

            // Can weapon switch again
            m_CanSwitchWeapon = true;

            // Animation after reloading sync through the network
            photonView.RPC("AfterReloadAnimation", PhotonTargets.All, m_SliderLockbacked);

            // After slide reset its position
            m_SliderLockbacked = false;

            // A temporary way to update the UI
            GameUIManager.Instance.GameInfoUI.UpdateAmmoCountUI(m_IsReloading,
                m_WeaponData.WeaponName,
                m_WeaponData.BulletsLeft,
                m_WeaponData.MagazineSize);
        }

        [PunRPC]
        private void ReloadingAnimation()
        {
            // Play remove mag sound
            m_WeaponSound.Magazine.PlayOneShot(m_WeaponSound.RemoveMag);

            // Play weapon reload aimation.
            m_Animation.WeaponMainFrame.Play(m_Animation.Reload);
            m_Animation.NewMagazine.Play(m_Animation.FullMagInsert);
        }

        [PunRPC]
        private void AfterReloadAnimation(bool isSliderLockbacked)
        {
            // Only play the pistol weapon snap forward animation if 
            // reload after the gun is completely ran out of ammo.
            if (m_SliderLockbacked)
            {
                m_Animation.WeaponSlide.Play(m_Animation.SlideCocking);
                // Play main reload sound
                m_WeaponSound.Reloading.PlayOneShot(m_WeaponSound.SlideSnap);
            }
            else
            {
                m_Animation.WeaponSlide.Play(m_Animation.SlideReload);
                m_WeaponSound.Sliding.PlayOneShot(m_WeaponSound.SlideReloadSound);
            }
        }
        #endregion
    }
}

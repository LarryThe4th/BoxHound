﻿using UnityEngine;
using System.Collections;
using BoxHound.UI;

namespace BoxHound
{
    public class Pistol : Weapon
    {
        #region Public classes
        [System.Serializable]
        private class WeaponHolders {
            [Header("Handgun Holders")]
            [Tooltip("The root of the weapon object.")]
            public Transform Holder;
            [Tooltip("The handgun's slide.")]
            public MeshRenderer Slide;
            [Tooltip("The handgun's main frame.")]
            public MeshRenderer MainFrame;
            [Tooltip("The magazine that is alread inserted in the weapon, " +
                "when reload this will become invisible untill the reload animation finished.")]
            public MeshRenderer LoadedMagazine;
            [Tooltip("Used in reloading animating and only visible durling reload.")]
            public MeshRenderer NewFullMagzine;

            [Header("Particle System")]
            // Particle system
            public ParticleSystem SmokeParticles;
        }
        [System.Serializable]
        private class SpawnPointsForAutomaticWeapon
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
            [Tooltip("Pull out the gun.")]
            public string PullOut;
            [Tooltip("Pull back the gun.")]
            public string PullBack;
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

        #region Private variables.
        // ------------ Private variables ------------
        [SerializeField]
        private WeaponHolders m_WeaponHolder;
        [SerializeField]
        private Animations m_Animation;
        [SerializeField]
        private WeaponSounds m_WeaponSound;
        [SerializeField]
        private SpawnPointsForAutomaticWeapon m_SpawnPoint;

        //For the weapon animation
        private float m_StartReloadTimeStamp = 0.0f;
        private bool m_HasPlayedOutOfAmmoAnimation = false;
        private bool m_SliderLockbacked = false;

        private bool m_Pullingout = false;
        private bool m_PullingBack = false;
        #endregion

        public override void Init(bool isDefaultWeapon)
        {
            base.Init(isDefaultWeapon);
            // Reset the time stamp.
            m_StartReloadTimeStamp = 0.0f;
            // The weapon is not even fired yet, of cause is FALSE.
            m_HasPlayedOutOfAmmoAnimation = false;
            // Same as above.
            m_SliderLockbacked = false;

            m_Pullingout = false;
            m_PullingBack = false;

            m_OneRoundRemainInChamber = false;
        }

        // Reset the weapon when player respawn or recover form ammo crate.
        public override void Reset()
        {
            // Refill the clips and ammo
            m_BulletsLeftInClip = m_WeaponData.MagazineSize;
            m_MagazineLeft = m_WeaponData.MagazineCount;
            m_RestBulletsCount = m_WeaponData.MagazineSize * m_MagazineLeft;

            // Clear the flag
            m_StartReloadTimeStamp = 0.0f;
            m_HasPlayedOutOfAmmoAnimation = false;
            m_SliderLockbacked = false;

            m_OneRoundRemainInChamber = false;

            m_Pullingout = false;
            m_PullingBack = false;

            // Update the weapon UI
            m_WeaponInfo.UpdateWeaponInfo(AmmoState.Normal,
                    m_WeaponData.WeaponName,
                    m_BulletsLeftInClip,
                    m_RestBulletsCount,
                    m_WeaponData.MagazineSize,
                    m_DefaultWeapon);

            DisplayWeaponModel(false);
        }

        /// <summary>
        /// Control the weapon renderer to show or hide the weapon.
        /// </summary>
        /// <param name="show">Set TRUE if want to show the weapon.</param>
        public override void DisplayWeaponModel(bool show)
        {
            m_WeaponHolder.Slide.enabled = show;
            m_WeaponHolder.MainFrame.enabled = show;
            m_WeaponHolder.LoadedMagazine.enabled = show;
        }

        // Process in every frame.        
        public override void Process()
        {
            // During holding the weapon.
            if (m_Phase == WeaponHoldingPhase.Holding)
            {
                #region Firing
                // If the user just click right mouse button
                // while the gun is not out of ammo and its not durling reloading.
                if (Input.GetMouseButtonDown(0) && !m_OutOfAmmo && !m_IsReloading)
                {
                    // Shot Animation sync through the network to all remote character.
                    photonView.RPC("WeaponFireAnimation", PhotonTargets.All);

                    // Cast a ray as the bullet.
                    RayCastSingleBullet(CharacterManager.LocalPlayer.MainCamera.transform);

                    // Reduce one round form the magazine/clip after shooting and
                    // update the local client's UI.
                    ReduceOneRound();
                }
                #endregion

                #region If out of ammo
                if (m_BulletsLeftInClip == 0 && !m_IsReloading)
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
                // If user press R to try reload the weapon.
                if (Input.GetKeyDown(KeyCode.R)) { 
                    // If the clip is not full and currently is not in the middle of reloading process.
                    if ( m_BulletsLeftInClip <= m_WeaponData.MagazineSize && !m_IsReloading && m_RestBulletsCount > 0) {
                        // The reload animation will sync through the network, else
                        // will be only effect on loacl client.
                        OnReloading();
                    }
                }
                #endregion
            }

            #region Right mouse button
            // If holding right mouse button
            if (Input.GetMouseButton(1))
            {
                // If not yet pulled out
                if (!m_Pullingout && m_Phase == WeaponHoldingPhase.Hiding)
                {
                    // The weapon now is pulling out.
                    m_Pullingout = true;
                    m_Phase = WeaponHoldingPhase.PullingOut;

                    // Show corss hair.
                    m_CorssHair.ShowCorssHair(true);

                    // photonView.RPC("PullOut", PhotonTargets.All);
                    photonView.RPC("PullOut", PhotonTargets.All);

                    // After waiting the pull out animation finished, the gun now is pulled out.
                    m_Phase = WeaponHoldingPhase.Holding;
                    m_Pullingout = false;
                }
            }
            // If let go the right mouse button
            else if (!m_PullingBack && m_Phase == WeaponHoldingPhase.Holding)
            {
                m_PullingBack = true;
                // If player try to hide the weapon when the gun is still reloading.
                if (m_IsReloading)
                {
                    // wait unity reload finish and then pull it back.
                    Invoke("PullbackDelay", (m_WeaponData.ReloadDuration + .5f) - m_StartReloadTimeStamp);
                }
                else
                {
                    PullbackDelay();
                }
            }
            #endregion

            // Calculate how long did the reloading process last.
            if (m_IsReloading)
            {
                m_StartReloadTimeStamp += Time.deltaTime;
            }
        }

        #region When pull in or pull out weapon

        [PunRPC]
        private void PullOut() {
            // Now the weapon is visiable.
            DisplayWeaponModel(true);

            // Player the pull out animation.
            m_Animation.WeaponMainFrame.Play(m_Animation.PullOut);
        }

        /// <summary>
        /// Wait until the reloading animation finished then pullback
        /// </summary>
        private void PullbackDelay()
        {
            // Fadeout the corss hair. 
            // This should only affect the local client.
            m_CorssHair.ShowCorssHair(false);

            // The weapon now is pulling out.
            m_Phase = WeaponHoldingPhase.PullingBack;

            photonView.RPC("PullBack", PhotonTargets.All);
        }

        [PunRPC]
        private void PullBack() {
            // Player the pull back animation.
            m_Animation.WeaponMainFrame.Play(m_Animation.PullBack);
        }

        /// <summary>
        /// A function for force pulling back the weapon. 
        /// Usefull in cases such as opening the game menu or opening the team select menu.
        /// </summary>
        public override void ForcePullBackWeapon()
        {
            if (m_Phase == WeaponHoldingPhase.Holding || m_Phase == WeaponHoldingPhase.PullingOut) {
                // If player try to hide the weapon when the gun is still reloading.
                if (m_IsReloading)
                {
                    // wait unity reload finish and then pull it back.
                    Invoke("PullbackDelay", m_WeaponData.ReloadDuration - m_StartReloadTimeStamp);
                }
                else
                {
                    PullbackDelay();
                }
            }
        }

        // Animation event
        public void PulledBack() {
            // Hide the weapon
            DisplayWeaponModel(false);

            m_PullingBack = false;

            // After waiting the pull back animation finished, the gun now is hided.
            m_Phase = WeaponHoldingPhase.Hiding;
        }
        #endregion

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
            m_WeaponSound.Firing.pitch = UnityEngine.Random.Range(.9f, 1.1f);
            m_WeaponSound.Firing.PlayOneShot(m_WeaponSound.SigleShotSound);

            // Play smoke particles
            m_WeaponHolder.SmokeParticles.Play();

            // Spawn casing 
            //TODO: pooling
            Instantiate(m_SpawnPoint.CasingPrefab, m_SpawnPoint.CasingSpawnPoint.position,
                            m_SpawnPoint.CasingSpawnPoint.rotation);
        }

        /// <summary>
        /// After the shot animation, reduce one round on the magazine.
        /// Do the math and update the UI only in local client.
        /// </summary>
        private void ReduceOneRound() {
            // Remove one bullet everytime you shoot
            m_BulletsLeftInClip -= 1;

            // Update weapon UI.
            m_WeaponInfo.UpdateWeaponInfo(
                // If both the clip in the gun is empty and no bullets left for reload.
                (m_RestBulletsCount == 0 && m_BulletsLeftInClip == 0) ? AmmoState.OutOfAmmo : AmmoState.Normal,
                m_WeaponData.WeaponName,
                m_BulletsLeftInClip,
                m_RestBulletsCount,
                m_WeaponData.MagazineSize,
                m_DefaultWeapon);
        }
        #endregion

        #region When weapon dry
        // Only set flags in local client, 
        // in other player's scene show only the animation is enough.
        private void OnWeaponDry()
        {
            // Update the local client's UI.
            m_WeaponInfo.UpdateWeaponInfo(
                // If both the clip in the gun is empty and no bullets left for reload.
                (m_RestBulletsCount == 0 && m_BulletsLeftInClip == 0) ? AmmoState.OutOfAmmo : AmmoState.ClipOutOfAmmo,
                m_WeaponData.WeaponName,
                m_BulletsLeftInClip,
                m_RestBulletsCount,
                m_WeaponData.MagazineSize,
                m_DefaultWeapon);

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
            // Start reload process.
            StartCoroutine(Reload());
        }

        // The reloading process.
        private IEnumerator Reload()
        {
            // Reset the reload time stamp.
            m_StartReloadTimeStamp = 0.0f;

            // Can't switch weapon while reloading.
            m_CanSwitchWeapon = false;

            // Start weapon reloading sequence.
            m_IsReloading = true;

            // Update the weapon UI.
            m_WeaponInfo.UpdateWeaponInfo(AmmoState.Reloading,
            m_WeaponData.WeaponName,
            m_BulletsLeftInClip,
            m_RestBulletsCount,
            m_WeaponData.MagazineSize,
            m_DefaultWeapon);

            // Reloading Animation sync through the network
            photonView.RPC("ReloadingAnimation", PhotonTargets.All);

            // Wait for set amount of time, this time is based on the length of the reload animation.
            yield return new WaitForSeconds(1.2f);

            // Play insert mag sound
            m_WeaponSound.Magazine.PlayOneShot(m_WeaponSound.InsertMag);

            // Wait for set amount of time
            yield return new WaitForSeconds(.4f);

            // The reload automatic pistol.
            AutomaticTypeWeaponReload();

            // Reload complete, weapon is hot.
            m_HasPlayedOutOfAmmoAnimation = false;
            m_OutOfAmmo = false;
            m_IsReloading = false;

            // Animation after reloading sync through the network
            photonView.RPC("AfterReloadAnimation", PhotonTargets.All, m_SliderLockbacked);

            // After slide reset its position
            m_SliderLockbacked = false;

            // Update weapon UI.
            m_WeaponInfo.UpdateWeaponInfo(
                // If both the clip in the gun is empty and no bullets left for reload.
                (m_RestBulletsCount == 0 && m_BulletsLeftInClip == 0) ? AmmoState.OutOfAmmo : AmmoState.Normal,
                m_WeaponData.WeaponName,
                m_BulletsLeftInClip,
                m_RestBulletsCount,
                m_WeaponData.MagazineSize,
                m_DefaultWeapon);

            // Can weapon switch again
            m_CanSwitchWeapon = true;
        }

        [PunRPC]
        private void ReloadingAnimation()
        {
            // Play remove mag sound
            m_WeaponSound.Magazine.PlayOneShot(m_WeaponSound.RemoveMag);

            // Play weapon reload aimation.
            m_Animation.WeaponMainFrame.Play(m_Animation.Reload);
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
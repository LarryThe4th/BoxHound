using UnityEngine;
using System.Collections;
using BoxHound.UI;

namespace BoxHound
{
    public class SwanOffShotgun : Weapon
    {
        #region Public classes
        [System.Serializable]
        private class WeaponHolders
        {
            [Header("Sawed-off shotgun Holders")]
            [Tooltip("The root of the weapon object.")]
            public Transform Holder;
            [Tooltip("The Sawed-off shotgun frame.")]
            public MeshRenderer MainFrame;
            [Tooltip("The Sawed-off shotgun barrel.")]
            public MeshRenderer Barrel;

            public MeshRenderer NewRoundRight = null;
            public MeshRenderer NewRoundLeft = null;

            public MeshRenderer AmmoInRightBarrel = null;
            public MeshRenderer AmmoInLeftBarrel = null;

            [Header("Particle System")]
            // Particle system
            public ParticleSystem SmokeParticles = null;
            public ParticleSystem SparkParticles = null;
        }

        [System.Serializable]
        private class SwpanPointsForSawedOffShotgun
        {
            [Header("Sawn Off Shotgun")]
            [Tooltip("The Sawed-off shotgun's barrel.")]
		    public GameObject Barrels = null;
            [Tooltip("The prefab of Sawed-off shotgun rounds.")]
            public GameObject ShotgunRoundPrefab = null;
            [Tooltip("The ammo in right barrel.")]
		    public GameObject AmmoInRightBarrel = null;
            [Tooltip("The ammo in left barrel.")]
            public GameObject AmmoInLeftBarrel = null;
            [Tooltip("Ammo for the New insert ammo to right barrel animation.")]
            public GameObject NewRoundRight = null;
            [Tooltip("Ammo for the New insert ammo to left barrel animation.")]
            public GameObject NewRoundleft = null;
            public Transform CasingSpawnPointRight = null;
            public Transform CasingSpawnPointLeft = null;
        }

        [System.Serializable]
        private class Animations
        {
            [Header("Pistol Animation compoment")]
            [Tooltip("The animation compoment on the Sawd-Off Shotgun's holder.")]
            public Animation WeaponHolder;
            [Tooltip("The animation compoment on the Sawd-Off Shotgun's barrels.")]
            public Animation WeaponBarrels;

            [Tooltip("The animation compoment on the ammo which is in the right barrel.")]
            public Animation NewRoundToRightBarrel;
            [Tooltip("The animation compoment on the ammo which is in the left barrel.")]
            public Animation NewRoundToLeftBarrel;

            [Header("Sawd-Off Shotgun Animation clip name")]
            // Sawd-Off Shotgun animations
            [Tooltip("The recoil animtion when firing the weapon.")]
            public string Recoil;
            [Tooltip("Whipping down the weapon for opening the barrel so it can reject the empty case")]
            public string ReloadWhippingDown;
            [Tooltip("Whipping up the weapon for closing the barrel after reloaded.")]
            public string ReloadedWhippingUp;
            [Tooltip("Open the barrel to eject the empty case.")]
            public string ReloadOpenBarrel;
            [Tooltip("Open the barrel to finish the reloading process.")]
            public string ReloadedCloseBarrel;

            [Tooltip("Insert new round to the righe barrel.")]
            public string InsertAmmoToRight;
            [Tooltip("Insert new round to the left barrel.")]
            public string InsertAmmoToLeft;


            [Tooltip("Pull out the gun.")]
            public string PullOut;
            [Tooltip("Pull back the gun.")]
            public string PullBack;
        }

        [System.Serializable]
        private class WeaponSounds
        {
            [Tooltip("Shooting Sounds")]
            [Header("Weapon Sound source")]
            public AudioSource Firing = null;
            public AudioSource Reloading = null;
            public AudioSource Foley = null;

            [Header("Firing Sounds")]
            [Tooltip("Main fire sound")]
            public AudioClip SigleShotSound;
            [Tooltip("The sound of clicking the trigger while out of ammo.")]
            public AudioClip DryFireSound;

            [Header("Reloading Sounds")]
            [Tooltip("Insert shotgun ammo")]
            public AudioClip IsertAmmo;
            [Tooltip("The sound of opening the Sawed-off shotgun's barrel.")]
            public AudioClip OpenBarrel;
            [Tooltip("The sound of closing the Sawed-off shotgun's barrel.")]
            public AudioClip CloseBarrel;

            [Header("Pullout Sounds")]
            [Tooltip("The sound of pulling the weapon out.")]
            public AudioClip pullout;
            [Tooltip("The sound of pulling the weapon back.")]
            public AudioClip pullback;
        }
        #endregion

        #region Variables.
        [SerializeField]
        private WeaponHolders m_WeaponHolder;
        [SerializeField]
        private Animations m_Animation;
        [SerializeField]
        private WeaponSounds m_WeaponSound;
        [SerializeField]
        private SwpanPointsForSawedOffShotgun m_SpawnPoint;

        //For the weapon animation
        private float m_StartReloadTimeStamp = 0.0f;
        private bool m_Pullingout = false;
        private bool m_PullingBack = false;
        #endregion

        public override void Init(bool isDefaultWeapon)
        {
            base.Init(isDefaultWeapon);
            // Reset the time stamp.
            m_StartReloadTimeStamp = 0.0f;
            m_Pullingout = false;
            m_PullingBack = false;
        }

        public override void DisplayWeaponModel(bool show)
        {
            m_WeaponHolder.MainFrame.enabled = show;
            m_WeaponHolder.Barrel.enabled = show;
            m_WeaponHolder.NewRoundRight.enabled = show;
            m_WeaponHolder.NewRoundLeft.enabled = show;
            m_WeaponHolder.AmmoInRightBarrel.enabled = show;
            m_WeaponHolder.AmmoInLeftBarrel.enabled = show;
        }

        public override void Reset()
        {
            // Refill the clips and ammo
            m_BulletsLeftInClip = m_WeaponData.MagazineSize;
            m_MagazineLeft = m_WeaponData.MagazineCount;
            m_RestBulletsCount = m_WeaponData.MagazineSize * m_MagazineLeft;

            // Clear the flag
            m_Pullingout = false;
            m_PullingBack = false;

            // Update the weapon UI
            GameRoomUI.Instance.PlayerHUDUI.UpdateAmmoCountUI(PlayerHUD.AmmoState.Normal,
                    m_WeaponData.WeaponName,
                    m_BulletsLeftInClip,
                    m_RestBulletsCount,
                    m_WeaponData.MagazineSize,
                    m_DefaultWeapon);

            DisplayWeaponModel(false);
        }

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
                    // TODO: This need a shotgun specific raycast! IT IS A MUST!
                    RayCastSingleBullet(CharacterManager.LocalPlayer.MainCamera.transform);

                    // Reduce one round form the magazine/clip after shooting and
                    // update the local client's UI.
                    ReduceOneRound();
                }
                #endregion

                #region If out of ammo
                if (m_BulletsLeftInClip == 0 && !m_IsReloading)
                {
                    // Set flags on local client.
                    OnWeaponDry();
                }
                #endregion

                #region Reloading
                // If user press R to try reload the weapon.
                if (Input.GetKeyDown(KeyCode.R))
                {
                    // If the clip is not full and currently is not in the middle of reloading process.
                    if (m_BulletsLeftInClip <= m_WeaponData.MagazineSize && !m_IsReloading && m_RestBulletsCount > 0)
                    {
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

        #region When pull back or pull out weapon
        [PunRPC]
        private void PullOut()
        {
            // Now the weapon is visiable.
            DisplayWeaponModel(true);

            // Player the pull out animation.
            m_Animation.WeaponHolder.Play(m_Animation.PullOut);

            m_WeaponSound.Foley.PlayOneShot(m_WeaponSound.pullout);
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
        private void PullBack()
        {
            // Player the pull back animation.
            m_Animation.WeaponHolder.Play(m_Animation.PullBack);

            m_WeaponSound.Foley.PlayOneShot(m_WeaponSound.pullback);
        }

        /// <summary>
        /// A function for force pulling back the weapon. 
        /// Usefull in cases such as opening the game menu or opening the team select menu.
        /// </summary>
        public override void ForcePullBackWeapon()
        {
            if (m_Phase == WeaponHoldingPhase.Holding || m_Phase == WeaponHoldingPhase.PullingOut)
            {
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
        public void PulledBack()
        {
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
        private void WeaponFireAnimation()
        {
            // Show muzzle flash
            StartCoroutine(MuzzleFlash());

            // Player weapon recoil animation.
            m_Animation.WeaponHolder.Play(m_Animation.Recoil);

            // Play firing sound with a different tones every time it shoots.
            m_WeaponSound.Firing.pitch = UnityEngine.Random.Range(.9f, 1.1f);
            m_WeaponSound.Firing.PlayOneShot(m_WeaponSound.SigleShotSound);

            // Play smoke and spark particles
            m_WeaponHolder.SmokeParticles.Play();
            m_WeaponHolder.SparkParticles.Play();
        }

        /// <summary>
        /// After the shot animation, reduce one round on the magazine.
        /// Do the math and update the UI only in local client.
        /// </summary>
        private void ReduceOneRound()
        {
            // Remove one bullet everytime you shoot
            m_BulletsLeftInClip -= 1;

            // Update weapon UI.
            GameRoomUI.Instance.PlayerHUDUI.UpdateAmmoCountUI(
                // If both the clip in the gun is empty and no bullets left for reload.
                (m_RestBulletsCount == 0 && m_BulletsLeftInClip == 0) ? PlayerHUD.AmmoState.OutOfAmmo : PlayerHUD.AmmoState.Normal,
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
            GameRoomUI.Instance.PlayerHUDUI.UpdateAmmoCountUI(
                // If both the clip in the gun is empty and no bullets left for reload.
                (m_RestBulletsCount == 0 && m_BulletsLeftInClip == 0) ? PlayerHUD.AmmoState.OutOfAmmo : PlayerHUD.AmmoState.ClipOutOfAmmo,
                m_WeaponData.WeaponName,
                m_BulletsLeftInClip,
                m_RestBulletsCount,
                m_WeaponData.MagazineSize,
                m_DefaultWeapon);

            // Flag the weapon as out of ammo.
            m_OutOfAmmo = true;

            // When user clicked right mouse button while the weapon is dry and not durling reloading.
            if (Input.GetMouseButtonDown(0) && !m_IsReloading)
            {
                //Play dry fire sound if clicking when out of ammo
                m_WeaponSound.Firing.PlayOneShot(m_WeaponSound.DryFireSound);
            }
        }
        #endregion

        #region When reloading
        // Start reload process.
        private void OnReloading()
        {
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
            GameRoomUI.Instance.PlayerHUDUI.UpdateAmmoCountUI(PlayerHUD.AmmoState.Reloading,
            m_WeaponData.WeaponName,
            m_BulletsLeftInClip,
            m_RestBulletsCount,
            m_WeaponData.MagazineSize,
            m_DefaultWeapon);

            // Reloading Animation sync through the network
            photonView.RPC("StartReloadingAnimation", PhotonTargets.All);

            // Wait for set amount of time, this time is based on the length of the reload animation.
            yield return new WaitForSeconds(m_WeaponData.ReloadDuration);

            // Reload ammo.
            SawedOffShotgunReload();

            // Finish reloading Animation sync through the network
            photonView.RPC("StartFinishingReloadAnimation", PhotonTargets.All);

            // Update weapon UI.
            GameRoomUI.Instance.PlayerHUDUI.UpdateAmmoCountUI(
                // If both the clip in the gun is empty and no bullets left for reload.
                (m_RestBulletsCount == 0 && m_BulletsLeftInClip == 0) ? PlayerHUD.AmmoState.OutOfAmmo : PlayerHUD.AmmoState.Normal,
                m_WeaponData.WeaponName,
                m_BulletsLeftInClip,
                m_RestBulletsCount,
                m_WeaponData.MagazineSize,
                m_DefaultWeapon);
        }

        [PunRPC]
        // There is a little delay between opening the barrel and inserting the ammo.
        private void StartReloadingAnimation() {
            StartCoroutine(ReloadingAnimation());
        }

        private IEnumerator ReloadingAnimation()
        {
            // Hide the ammo that are in the barrel.
            m_WeaponHolder.AmmoInRightBarrel.enabled = false;
            m_WeaponHolder.AmmoInLeftBarrel.enabled = false;

            // Play the reload animation, including the weapon whipping down 
            // and opeing the barrel.
            m_Animation.WeaponHolder.Play(m_Animation.ReloadWhippingDown);
            m_Animation.WeaponBarrels.Play(m_Animation.ReloadOpenBarrel);

            // Play barrels open sound
            m_WeaponSound.Reloading.PlayOneShot(m_WeaponSound.OpenBarrel);

            // Wait some time
            yield return new WaitForSeconds(0.15f);

            // Spawn empty casing prefabs
            Instantiate(m_SpawnPoint.ShotgunRoundPrefab, 
                        m_SpawnPoint.CasingSpawnPointRight.position,
                        m_SpawnPoint.CasingSpawnPointRight.rotation);

            Instantiate(m_SpawnPoint.ShotgunRoundPrefab,
                m_SpawnPoint.CasingSpawnPointLeft.position,
                m_SpawnPoint.CasingSpawnPointLeft.rotation);


            m_Animation.NewRoundToRightBarrel.Play();

            if (m_RestBulletsCount > 1)
            {
                m_Animation.NewRoundToLeftBarrel.Play();
            }
            else {
                m_WeaponHolder.NewRoundLeft.enabled = false;
            }
        }

        [PunRPC]
        // There is a little delay between opening the barrel and inserting the ammo.
        private void StartFinishingReloadAnimation()
        {
            StartCoroutine(AfterReloadAnimation());
        }

        private IEnumerator AfterReloadAnimation()
        {
            // Play the reload animation, including the weapon whipping up 
            // and closing the barrel.
            m_Animation.WeaponHolder.Play(m_Animation.ReloadedWhippingUp);
            m_Animation.WeaponBarrels.Play(m_Animation.ReloadedCloseBarrel);

            // Play barrels close sound
            m_WeaponSound.Reloading.PlayOneShot(m_WeaponSound.CloseBarrel);

            // Wait until animations are finished
            yield return new WaitForSeconds(0.12f);

            // Set the shellcasings visible again.
            m_WeaponHolder.AmmoInRightBarrel.enabled = true;
            m_WeaponHolder.AmmoInLeftBarrel.enabled = true;

            //Enable shooting again
            m_OutOfAmmo = false;
            m_IsReloading = false;

            m_CanSwitchWeapon = true;
        }
        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BoxHound.UI {
    public class PlayerHUD : RoomUIBase
    {
        #region Public variables
        // The perfab of the floating damage number
        public PoolObject FloatingNumberPerfab;
        public RespawnCountDown RespawnCountDownUI;
        public KillComfirmation KillConfirmUI;
        // public RoundInfo RoundInfoUI;
        public CorssHairManager CorssHair;

        public CanvasGroup WeaponInfo;
        public CanvasGroup HealthBar;

        public enum AmmoState {
            Normal,
            Reloading,
            ClipOutOfAmmo,
            OutOfAmmo,
        }
        #endregion

        #region Private variables
        // -------------- Public variable -------------
        [SerializeField]
        private CanvasGroup m_AmmoCountCanvasGroup; // The ammo count UI.
        [SerializeField]
        private CanvasGroup m_AmmoStateCanvasGroup; // The Reloading UI.

        [SerializeField]
        private Transform m_FloatingNumberRoot;

        // The instance ID of the pooling object.
        private int m_PoolObjectID = 0;

        private DamageIndicatorManager m_DamageIndicatorManager;

        [Header("Health bar UIs")]
        [SerializeField]
        private Text m_HealthPoints;
        [SerializeField]
        private Image m_HealthBarTop;

        [Header("Ammo count UIs")]
        [SerializeField]
        private Text m_AmmoInClipText;
        [SerializeField]
        private Text m_TotalBulletLeft;
        [SerializeField]
        private Text m_WeaponName;
        [SerializeField]
        private Text m_AmmoStateText;
        #endregion

        public override void DisplayUI(bool show)
        {
            base.DisplayUI(show);
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void Init() {
            DisplayUI(false);

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

            m_AmmoCountCanvasGroup.alpha = 1;
            m_AmmoStateCanvasGroup.alpha = 0;

            m_DamageIndicatorManager = GetComponentInChildren<DamageIndicatorManager>();
            if (!m_DamageIndicatorManager) Debug.Log("Can't find damage indicator manager.");

            RespawnCountDownUI = GetComponentInChildren<RespawnCountDown>();
            if (!RespawnCountDownUI) Debug.Log("Can't find respawn count down script.");

            KillConfirmUI = GetComponentInChildren<KillComfirmation>();
            if (!KillConfirmUI) Debug.Log("Can't find kill comfirmation script.");

            //RoundInfoUI = GetComponentInChildren<RoundInfo>();
            //if (!RoundInfoUI) Debug.Log("Can't find kill RoundInfo script.");

            CorssHair = GetComponentInChildren<CorssHairManager>();
            if (!CorssHair) Debug.Log("Can't find CorssHairManager script.");

            CorssHair.Init();
        }

        public void DisplayWeaponInfoAndHeathBar(bool show) {
            if (show)
            {
                WeaponInfo.alpha = 1;
                HealthBar.alpha = 1;
            }
            else {
                WeaponInfo.alpha = 0;
                HealthBar.alpha = 0;
            }
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
                    UnityEngine.Random.Range(screenPosition.x - 20f, screenPosition.x + 20f),
                    UnityEngine.Random.Range(screenPosition.y, screenPosition.y + 20f),
                    0), Quaternion.identity,
                new object[1] { damage });
        }

        public void OnHealthUpdate(int maximunHealth, int currentHealthPoint) {
            m_HealthPoints.text = currentHealthPoint.ToString();
            m_HealthBarTop.fillAmount = (float)currentHealthPoint / maximunHealth;
        }

        public void UpdateAmmoCountUI(AmmoState state, string weaponName, int roundsInClip, int totalLeftRounds, int magazineSize, bool isDefaultWeapon = false) {
            m_WeaponName.text = weaponName;

            switch (state) {
                case AmmoState.Normal:
                    m_AmmoCountCanvasGroup.alpha = 1;
                    m_AmmoStateCanvasGroup.alpha = 0;
                    // Show white as usualy
                    m_AmmoInClipText.color = Color.white;
                    break;
                case AmmoState.Reloading:
                    m_AmmoCountCanvasGroup.alpha = 0;
                    m_AmmoStateCanvasGroup.alpha = 1;

                    m_AmmoStateText.text = "リロード中";
                    m_AmmoStateText.color = Color.yellow;
                    break;
                case AmmoState.ClipOutOfAmmo:
                    m_AmmoCountCanvasGroup.alpha = 1;
                    m_AmmoStateCanvasGroup.alpha = 0;
                    // Set the number to red so player can notice it.
                    m_AmmoInClipText.color = Color.red;

                    break;
                case AmmoState.OutOfAmmo:
                    m_AmmoCountCanvasGroup.alpha = 0;
                    m_AmmoStateCanvasGroup.alpha = 1;
                    m_AmmoStateText.text = "弾切れ";
                    m_AmmoStateText.color = Color.red;
                    break;
                default:
                    m_AmmoCountCanvasGroup.alpha = 1;
                    m_AmmoStateCanvasGroup.alpha = 0;
                    // Show white as usualy
                    m_AmmoInClipText.color = Color.yellow;
                    break;
            }

            m_AmmoInClipText.text = roundsInClip.ToString();
            if (roundsInClip > magazineSize)
                m_AmmoInClipText.color = Color.yellow;

            if (isDefaultWeapon)
                m_TotalBulletLeft.text = "∞";
            else
                m_TotalBulletLeft.text = totalLeftRounds.ToString();
        }

        public void ProcessHUD() {
            // If the respawn count down is displaying, 
            // the only reason is that the player is died.
            if (RespawnCountDownUI.IsDisplaying()) {
                RespawnCountDownUI.UpdateRespawnCountDown();
            }

            // RoundInfoUI.UdpateTimer();

            // Update the indicators
            m_DamageIndicatorManager.ProcessIndicators();
        }

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            return GameRoomUI.RoomUITypes.PlayerHUD;
        }
    }
}

using UnityEngine;
using Random = UnityEngine.Random;

using BoxHound.UI;

namespace BoxHound
{
    /// <summary>
    /// This is the main character class
    /// Basicly every about the character, health, damage, respawn,
    /// and synchronizing these values through the network is done in here.
    /// </summary>
    public class CharacterManager : Photon.PunBehaviour
    {
        #region Private variables
        // -------------- Private variable ------------
        /// <summary>
        /// The control of the character, only enable when this is the 
        /// character that the local client is contorlling, else will be
        /// disabled and let the photon viewer to synchronize the character movment
        /// </summary>
        private CharacterControl m_Control;

        /// <summary>
        /// A compoment came along with the original FPS contorller, 
        /// i will remove it and rewrite my own in the future.
        /// </summary>
        private CharacterController m_Controller;

        /// <summary>
        /// The weapon manager on this character, as its name, it managed
        /// all the available weapon in the game, since everyone is instantiated
        /// form a same character prefab, the available weapons are allways the same
        /// </summary>
        private WeaponManager m_WeaponManager;

        /// <summary>
        /// The damage handler of the character, all the damage is receive form here.
        /// </summary>
        private DamageHandler m_DamageHandler;

        /// <summary>
        /// The head-up display UI for the player.
        /// </summary>
        private PlayerHUDUI m_PlayerHUD;
        #endregion

        #region Public variables
        // -------------- Public variable -------------
        /// <summary>
        /// The one and the only local player on each client
        /// </summary>
        public static CharacterManager LocalPlayer;

        /// <summary>
        /// The renderer of the character's model.
        /// When player 
        /// </summary>
        public Renderer PlayerModel;

        /// <summary>
        /// Character handles damage through this.
        /// </summary>
        public DamageHandler HandleDamage {
            get { if (!m_DamageHandler) m_DamageHandler = GetComponentInChildren<DamageHandler>();
                return m_DamageHandler; 
            }
        }

        /// <summary>
        /// Get character weapon manager.
        /// </summary>
        public WeaponManager GetWeaponManager
        {
           get { if (!m_WeaponManager)
                    Helper.GetCachedComponent<WeaponManager>(this.gameObject, ref m_WeaponManager);
                return m_WeaponManager;
            }
        }

        /// <summary>
        /// This is camera act as the eye of the character.
        /// </summary>
        public Camera MainCamera;

        /// <summary>
        /// Is this character object visibale? Return TURN if visiable.
        /// </summary>
        public bool IsVisiable
        {
            get; private set;
        }

        /// <summary>
        /// Get which team did this character belongs to.
        /// </summary>
        public CustomRoomOptions.Team Team
        { get; private set; }

        /// <summary>
        /// The character's maximun health.
        /// </summary>
        public static int MaximunHealth;

        /// <summary>
        /// The character's current health.
        /// </summary>
        public int CurrentHealth {
            get; private set;
        }

        /// <summary>
        /// Get the playr HUD.
        /// </summary>
        public PlayerHUDUI GetPlayerHUD {
            get { if (!m_PlayerHUD)
                    m_PlayerHUD = UIframework.UIManager.Instance.GetUI<PlayerHUDUI>(UIframework.UIManager.SceneUIs.PlayerHUDUI);
                return m_PlayerHUD; }
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// These initizaltion will happen after the player object been instantiated or
        /// the scene reloaded and befer the character object's own Start() method;
        /// </summary>
        public void Init()
        {
            // Used in Room manager we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.isMine)
            {
                // Double check 
                LocalPlayer = this;

                // Use photon.
                PhotonNetwork.player.SetScore(0);

                // Change the name of the object so makes easier for debugging.
                this.gameObject.name = "Local player";

                // Change the tag to localPlayer as it won't be taged as "Target" and receive damage from itself.
                this.gameObject.tag = "LocalPlayer";

                // Set itself's layer to "player" as that the raycast will ignore this object's collider.
                this.gameObject.layer = LayerMask.NameToLayer("Player");

                // Temp
                if (MainCamera) {
                    UIframework.UIManager.Instance.ResetUnderBlurCanvasRenderCamera(MainCamera);
                }
            }

            // Apply the default value.
            // Reset the team info.
            Team = CustomRoomOptions.Team.OneManArmy;

            // Reset health
            MaximunHealth = (int)PhotonNetwork.room.customProperties[RoomProperties.HealthLimit];
            CurrentHealth = MaximunHealth;

            // Hide it untill respawn
            IsVisiable = false;

            Helper.GetCachedComponent<CharacterControl>(this.gameObject, ref m_Control);
            // That is werid, the CharacterController is not a part of MonoBehaviour's compoment.
            m_Controller = GetComponent<CharacterController>();
            Helper.GetCachedComponent<WeaponManager>(this.gameObject, ref m_WeaponManager);
            m_DamageHandler = GetComponentInChildren<DamageHandler>();
            m_PlayerHUD = GetPlayerHUD;

            // Initlize the weapon manager
            m_WeaponManager.Init();

            // Don't show the player HUD UI at the beginning.
            //if (photonView.isMine)
            //    UIframework.UIManager.Instance.hide(UIframework.UIManager.SceneUIs.PlayerHUDUI);

            // Enable the character contorl
            EnableCharacterControl(false);

            // Enable the unity3d controller compoment, without this the player character can't even move.
            m_Controller.enabled = false;

            HideCharacterModel(true);

            // Hide the player model and disable its contol for now until the game started.
            HideCharacterModel(photonView.isMine);
        }

        /// <summary>
        /// This is specific method that photon network use it to synchronize values over the network.
        /// It is usuful in cases such as health and other value that needs to be synchronize all the time.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Send and receive data
            SerializeState(stream, info);
        }

        private void SerializeState(PhotonStream stream, PhotonMessageInfo info)
        {
            // Send data
            if (stream.isWriting == true)
            {
                stream.SendNext(CurrentHealth);
            }
            // Receive data
            else
            {
                float oldHealth = CurrentHealth;
                CurrentHealth = (int)stream.ReceiveNext();

                //// If the health has not changed since last data recived, 
                //// no need to update it.
                //if (CurrentHealth != oldHealth)
                //{
                //    //Debug.Log("Here");
                //    //if (photonView.isMine)
                //    //    OnHealthChanged(new PhotonMessageInfo(PhotonNetwork.player, (int)PhotonNetwork.time, photonView));
                //}
            }
        }

        /// <summary>
        /// When eliminated an enemy, add score and player the eliminated comfirm animation.
        /// </summary>
        /// <param name="victim">The one you killed you bastard!</param>
        [PunRPC]
        private void OnEliminatedEnemy(PhotonMessageInfo info)
        {
            if (m_PlayerHUD == null) m_PlayerHUD = GetPlayerHUD;
            // Play kill confirm animation
            // TODO
            // m_PlayerHUD.KillConfirmUI.PlayKillConfirmAnimation();
        }

        [PunRPC]
        private void SyncTeam(int teamIndex, PhotonMessageInfo info)
        {
            Team = CustomRoomOptions.GetTeam(teamIndex);
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Use this method to synchronize every client which team this character's object is in.
        /// </summary>
        /// <param name="team"></param>
        public void SetTeam(CustomRoomOptions.Team team) {
            this.photonView.RPC("SyncTeam", PhotonTargets.AllBuffered, (int)team);
        }

        public void SetVisibility(bool visible) {
            IsVisiable = visible;
        }

        public void DealDamage(int damage, PhotonMessageInfo source, int weaponIndex = -1) {
            if (photonView.isMine) {
                // Reduce health
                CurrentHealth -= damage;
                m_PlayerHUD.GetHealthBar.UpdateHealthBar(MaximunHealth, CurrentHealth);
                OnHealthChanged(source, weaponIndex);
            }
        }

        /// <summary>
        /// When on health changed, hide the player on every client and
        /// inform the attacker about your death.
        /// <param name="source">The reson why the character's health changed.</param>
        /// <param name="weaponIndex">If this change is cause by a weapon, set the weapon index.</param>
        /// </summary>
        private void OnHealthChanged(PhotonMessageInfo source, int weaponIndex = -1)
        {
            // Check if current health is belew ZERO
            if (CurrentHealth <= 0)
            {
                // If health is below ZERO...That means you are died!
                // Health points can't go low than this point.
                CurrentHealth = 0;

                // Hide the player to somewhere other player can't reach.
                // Now you are turly at the six feet underground LOL.
                transform.position = new Vector3(
                    Random.Range(-100.0f, 100.0f),
                    -6.0f, Random.Range(-100.0f, 100.0f));

                // Hide weapon info UI and health bar when died.
                m_PlayerHUD.ShowIngameInfo(false);

                // Ensure no one sees you.
                SetVisibility(false);

                // Disable all the character related compomemts.
                m_Controller.enabled = false;
                EnableCharacterControl(false);
                EnableWeaponContorl(false);

                // Turn ON the scene camera.
                GameRoomManager.Instance.EnableSceneCamera(true);

                // Disable the character's main camera.
                EnableMainCamera(false);

                if (photonView.isMine == true)
                {
                    // Inform the attacker about your "tragedy".
                    // He/she will loves it i ensure you. 
                    photonView.RPC("OnEliminatedEnemy", source.sender);

                    GameRoomUI.Instance.PlayerHUDUI.RespawnCountDownUI.ShowRespawnCountDown(
                        source.sender.name, m_WeaponManager.GetWeaponByIndex(weaponIndex).GetWeaponName);

                    // Add score.
                    source.sender.AddScore(100);

                    this.photonView.RPC("OnDead", PhotonTargets.All, source.sender.name);

                    // Wait until the reapwn timer ends.
                    Invoke("Respawn", GameRoomManager.CurrentGameMode.GetRespawnTime);
                }
            }
        }

        public void Respawn() {
            // In case something died right befor round ended and yet respawn.
            if (GameRoomManager.CurrentGameMode.IsRoundFinished()) return;

            // Get a spawn point based on the team this character belongs to.
            Transform spawnPoint = GameRoomManager.CurrentGameMode.GetSpawnPoint(Team);

            // Show thw player HUD when respawn.
            //UIframework.UIManager.Instance.GetUI(UIframework.UIManager.SceneUIs.PlayerHUDUI).ShowUI();
            m_PlayerHUD.ShowUI();


            int number = UnityEngine.Random.Range(0, m_WeaponManager.WeaponListCount - 1);

            // Call all the character on every client that has this same photon view,
            // spawn to the given location.
            photonView.RPC("OnRespawn", photonView.owner, spawnPoint.position, spawnPoint.rotation, number);
        }

        [PunRPC]
        public void Color(PhotonMessageInfo info) {
            // If same team.
            if (Team == CustomRoomOptions.GetTeam((int)info.sender.customProperties[PlayerProperties.Team])) {
                if (Team == CustomRoomOptions.Team.Blue)
                {
                    PlayerModel.material.SetColor("_RimColor", UnityEngine.Color.blue);
                }
                else if (Team == CustomRoomOptions.Team.Red) {
                    PlayerModel.material.SetColor("_RimColor", UnityEngine.Color.red);
                } 
            } else {
                PlayerModel.material.SetColor("_RimColor", UnityEngine.Color.white);
            }
        }

        [PunRPC]
        public void OnDead(string killerName) {
            // GameRoomUI.Instance.MessageCenterUI.KillMessage(killerName, PhotonNetwork.player.name);
        }

        [PunRPC]
        public void OnRespawn(Vector3 spawnPosition, Quaternion spawnRotation, int weaponIndex)
        {
            // Set this character's respawn location.
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            // Reset the health points.
            CurrentHealth = MaximunHealth;

            // Update the health point.
            if (m_PlayerHUD == null) Debug.Log(">?");

            m_PlayerHUD.GetHealthBar.UpdateHealthBar(MaximunHealth, CurrentHealth);

            // Display weapon info UI and health bar after respawn.
            m_PlayerHUD.ShowIngameInfo(true);

            // Reset all the character related compomemts.
            m_Controller.enabled = true;
            EnableCharacterControl(true);
            EnableWeaponContorl(true);

            m_WeaponManager.SetWeapon(weaponIndex);

            // Enable the character's main camera.
            EnableMainCamera(true);

            // Turn off the scene camera.
            GameRoomManager.Instance.EnableSceneCamera(false);
        }

        /// <summary>
        /// This method gets called right after a GameObject is created 
        /// through PhotonNetwork.Instantiate.
        /// The fifth parameter in PhotonNetwork.instantiate sets what  
        /// ever data you passed in as the instantiation data.
        /// In here we use it to load the data form the loadout page.
        /// </summary>
        /// <param name="info">The instantiation data.</param>
        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            base.OnPhotonInstantiate(info);
        }

        /// <summary>
        /// Enable or disable the character's control.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableCharacterControl(bool enable)
        {
            m_Control.EnableCharacterContorl(enable);
        }

        /// <summary>
        /// Enable or disable the character's weapon,
        /// when disabling the weapon, it will force into the pull back animation and hide.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableWeaponContorl(bool enable)
        {
            m_WeaponManager.EnableWeapon(enable);
        }

        /// <summary>
        /// Control the character's camera active state.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableMainCamera(bool enable)
        {
            if (!MainCamera)
                foreach (var camera in GetComponentsInChildren<Camera>())
                {
                    if (camera.CompareTag("LocalPlayerCamera"))
                    {
                        MainCamera = camera;
                        break;
                    }
                }

            // Only enable the local player's camera.
            if (photonView.isMine)
                MainCamera.gameObject.SetActive(enable);
            else
                MainCamera.gameObject.SetActive(false);
        }

        /// <summary>
        /// When the game is not started yet or the player is waiting for respawn,
        /// hide the character and disable it.
        /// </summary>
        /// <param name="hide"></param>
        public void HideCharacterModel(bool hide)
        {
            PlayerModel.enabled = !hide;
        }
        #endregion
    }
}
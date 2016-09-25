using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    public class CharacterManager : Photon.PunBehaviour
    {
        #region Public variables
        // -------------- Public variable -------------
        public GameModeManager.Team AtTeam
            { get; private set; }

        public CharacterControl m_Control;
        public CharacterController m_Controller;
<<<<<<< HEAD
        
=======
>>>>>>> origin/master
        #endregion

        #region Private variables
        // -------------- Private variable ------------
        [Tooltip("The visual model of the player object, turn it off to hide the player.")]
        [SerializeField]
        private GameObject PlayerModel;

        [Tooltip("The main camera on the player as its eyes.")]
        [SerializeField]
        private Camera CharacterMainCamera;

        // When during new round peperation phase or waiting for respwan,
        // user can not control its character.
        private bool m_IsCharacterControllable;

        // The reference of the weapon manager.
        private WeaponManager m_WeaponManager = null;
        private CharacterControl m_CharacterControl = null;

        private DamageHandler m_DamageHandler;
        #endregion

        public Transform GetMainCameraTransform {
            get { return CharacterMainCamera.transform; } 
        }

<<<<<<< HEAD
        public Camera GetMainCamera
        {
            get { return CharacterMainCamera; }
        }

=======
>>>>>>> origin/master
        // This is for not only the local player but all the player object in scene.
        // When a local player join a room which is already some other players in it,
        // other player's game object will not initalize by the RoomManager because 
        // they are not the loacl player, so these game object only initalize in here.
        public void Start() {
            if (!m_WeaponManager)
                m_WeaponManager = GetComponent<WeaponManager>();

            if (!m_CharacterControl)
                m_CharacterControl = GetComponent<CharacterControl>();

            if (!m_DamageHandler)
                m_DamageHandler = GetComponentInChildren<DamageHandler>();

            // Hide the player model and disable its contol for now until the game started.
            if (photonView.isMine)
            {
                HideCharacter(true);
            }
            else {
                // We don't need any character control on a remote character object.
                // The PhotonView will synchronize its position and rotation.
                if (!m_Control) m_Control = GetComponent<CharacterControl>();
                m_Control.enabled = false;
                if (!m_Controller) m_Controller = GetComponent<CharacterController>();
                m_Controller.enabled = false;
            }

            // Disable the character's main character.
            EnableMainCamera(false);
        }

        /// <summary>
        /// These initizaltion will happen after the player object been Instantiated and befer the character object's own Start();
        /// </summary>
        public void Init() {
            if (!m_Control) m_Control = GetComponent<CharacterControl>();
            if (!m_Controller) m_Controller = GetComponent<CharacterController>();

            // Used in Room manager we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.isMine)
            {
                // Double check 
                RoomManager.LocalPlayer = this;

                // Change the name of the object so makes easier for debugging.
                this.gameObject.name = "Local player";

                // Change the tag to localPlayer as it won't be taged as "Target" and receive damage from itself.
                this.gameObject.tag = "LocalPlayer";

                this.gameObject.layer = LayerMask.NameToLayer("Player");

                // Enable the character contorl
                m_Control.enabled = true;
                m_Controller.enabled = true;

                AtTeam = GameModeManager.GetTeam((int)PhotonNetwork.player.customProperties[PlayerProperties.Team]);

                // Hide the player model and disable its contol for now until the game started.
                HideCharacter(true);

                // We flag as don't destroy on load so that instance survives level synchronization, 
                // thus giving a seamless experience when levels load.
                DontDestroyOnLoad(this);
            }
        }

        public DamageHandler HandleDamage { get { return m_DamageHandler; } }

        public void EnableCharacterControl(bool enable) {
            m_WeaponManager.EnableWeapon(enable);
            m_CharacterControl.EnableCharacterContorl(enable);
        }

        /// <summary>
        /// Respwan the player to the given location.
        /// </summary>
        /// <param name="respwanLocation">Where the player will be respwan.</param>
        public void RespwanAndEnbaleControl(Transform respwanLocation) {
            // I should use deleget to do this...
            this.gameObject.transform.position = respwanLocation.position;
            this.gameObject.transform.rotation = respwanLocation.rotation;

            EnableMainCamera(true);
            EnableCharacterControl(true);

            m_WeaponManager.EnableWeapon(true);
            m_CharacterControl.EnableCharacterContorl(true);
        }

        /// <summary>
        /// Control the character's camera.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableMainCamera(bool enable) {
            // Only enable the local player's camera.
            if (photonView.isMine)
                CharacterMainCamera.gameObject.SetActive(enable);
            else
                CharacterMainCamera.gameObject.SetActive(false);
        }

        /// <summary>
        /// When the game is not started yet or the player is waiting for respawn,
        /// hide the character and disable it.
        /// </summary>
        /// <param name="hide"></param>
        public void HideCharacter(bool hide) {
            Debug.Log("Hide player" + hide);
            PlayerModel.SetActive(!hide);
        }
    }
}
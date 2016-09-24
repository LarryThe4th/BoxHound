using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class MainMenu : Photon.PunBehaviour
    {
        #region Public variables
        // -------------- Public variables -------------
        [Tooltip("The cache of text UI compoment.")]
        public Text GameVersionText;        // The cache of text UI compoment.

        [Tooltip("The cache of input field UI compoment.")]
        public InputField PlayerNameInput;  // The cache of input field UI compoment.

        [Tooltip("The cache of input label text compoment.")]
        public Text PlayerNameInputLabel;   // The cache of input label text compoment.

        [Tooltip("The cache of Login button UI compoment.")]
        public Button LoginButton;          // The cache of Login button UI compoment.

        [Tooltip("The RectTransform compoment of the loading ring Image.")]
        public RectTransform LoadingRing;   // The RectTransform compoment of the loading ring Image.
        #endregion

        #region private variables
        // -------------- private variables -------------
        private string m_PlayerName = "";   // The player name that user inputed.
        private const int m_PlayerNameCharacterLimit = 16;
        private Vector3 m_RotationEuler;    // The roration euler value of the loading ring sprite image.

        private bool ConnectingToServer = false;
        #endregion

        #region Default MonoBehaviour methods.
        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start() {
            // Check if the cache of input field UI compoment is null.
            if (!PlayerNameInput) {
                PlayerNameInput = GetComponentInChildren<InputField>();
                if (!PlayerNameInput) {
                    Debug.Log("Can't find the input field compoment under " + this.gameObject.name);
                    return;
                }
            }
            // Set character input limits.
            PlayerNameInput.characterLimit = m_PlayerNameCharacterLimit;
            PlayerNameInput.characterValidation = InputField.CharacterValidation.Alphanumeric;

            // Apply the game version text.
            if (!GameVersionText) {
                foreach (var text in GetComponentsInChildren<Text>())
                {
                    if (text.tag.CompareTo("GameVersionText") == 0) {
                        GameVersionText = text;
                        break;
                    }
                }
                if (!GameVersionText) {
                    Debug.Log("Can't find the Game version Text compoment under " + this.gameObject.name);
                    return;
                }
            }
            GameVersionText.text = NetWorkManager.Instance.GetGameVersion + " Ver";


            LoadingRing.gameObject.SetActive(false);

            LoginButton.onClick.AddListener(delegate { OnLoginButtonClicked(); });

            // Befer user input hes/hers player name, the login function should be disabled.
            LoginButton.interactable = false;
        }

        private void Update()
        {
            if (ConnectingToServer) {
                // Increment 30 degrees every second.
                m_RotationEuler += Vector3.forward * -90 * Time.deltaTime;
                LoadingRing.rotation = Quaternion.Euler(m_RotationEuler);
            }

            if (LoginButton.interactable) {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnLoginButtonClicked();
            }

            BreathingLightsStyleUIText(PlayerNameInputLabel);
        }
        #endregion

        #region Private methods
        private void BreathingLightsStyleUIText(Text text) {
            text.color = new Color(1, 1, 1, Mathf.PingPong(Time.time * 0.5f, 1));
        }
        #endregion

        #region Public methods
        public void SetPlayerName(string name) {
            // If the player name is null or empty, user can't login the game.
            LoginButton.interactable = string.IsNullOrEmpty(name) ? false : true;
            m_PlayerName = name;
        }

        public void OnLoginButtonClicked() {
            if (string.IsNullOrEmpty(m_PlayerName)) {
                Debug.Log("Player name can't be null or empty.");
                return;
            }
            ConnectingToServer = true;
            LoadingRing.gameObject.SetActive(true);
            LoginButton.gameObject.SetActive(false);

            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);

            NetWorkManager.Instance.Connect(m_PlayerName);
            LoginButton.interactable = false;
        }
        #endregion
    }
}

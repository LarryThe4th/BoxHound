using UnityEngine;
using UnityEngine.UI;
using BoxHound.SaveAndLoad;
using System;

namespace BoxHound.UI
{
    public class MainMenuUI : UIBase
    {
        #region private variable
        // --------------- private variable --------------
        [SerializeField]
        private InputField UserNickNameInputField;

        [SerializeField]
        private Text UserNickNameInputFieldTip;

        [SerializeField]
        private Button LoginButton;
        #endregion

        public override void InitUI()
        {
            base.InitUI();
            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.MainMenuUI,
                UIframework.UIManager.DisplayUIMode.NeverHide,
                UIframework.UIManager.UITypes.UnderBlurEffect,
                false, true);

#if UNITY_EDITOR
            if (!UserNickNameInputField) Debug.LogError("UserNickNameInputField is null");
            if (!LoginButton) Debug.LogError("LoginButton is null");
            if (!UserNickNameInputFieldTip) Debug.LogError("UserNickNameInputFieldTip is null");
#endif

            UserNickNameInputField.onEndEdit.AddListener(value => OnUserNickNameUpdate(value));
            
            LoginButton.onClick.AddListener(delegate { OnClickedLoginButton(); });
            LoginButton.interactable = false;

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            UserNickNameInputFieldTip.text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Main_NickNameInputFieldTip, language);
            LoginButton.GetComponentInChildren<Text>().text = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Main_LoginButtonText, language);
        }

        /// <summary>
        /// When user finished input his/hers nickname, check if 
        /// is empty or null, if not store the nickname into the PlayerPerf
        /// for later use.
        /// </summary>
        /// <param name="name">The user input.</param>
        private void OnUserNickNameUpdate(string name) {
            if (!string.IsNullOrEmpty(name))
            {
                LoginButton.interactable = true;
                UserLoaclDataManager.SetStringToPayerPerf(UserLoaclDataManager.UserNickNameKey, name);
            }
            else {
                LoginButton.interactable = false;
                UserLoaclDataManager.DeleteTheKey(UserLoaclDataManager.UserNickNameKey);
            }
        }

        private void OnClickedLoginButton()
        {
            // Double check the user input.
            if (UserLoaclDataManager.PlayerPerfabHasKey(UserLoaclDataManager.UserNickNameKey))
            {
                if (!string.IsNullOrEmpty(UserLoaclDataManager.GetstringFormPlayerPerf(UserLoaclDataManager.UserNickNameKey)))
                {
                    NetworkManager.Instance.ConnectToServer(
                        UserLoaclDataManager.GetstringFormPlayerPerf(UserLoaclDataManager.UserNickNameKey));
                }
                else {
                    LoginButton.interactable = false;
                }
            }
        }
    }
}

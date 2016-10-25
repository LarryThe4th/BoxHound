using UnityEngine;
using UnityEngine.UI;
using BoxHound.UIframework;

namespace BoxHound.UI
{
    public class ExitGameDialog : DialogUIBase
    {
        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.ExitGameDialogUI,
                UIframework.UIManager.DisplayUIMode.Dialog,
                UIframework.UIManager.UITypes.AboveBlurEffect,
                false, true, true);

            // Exit game dialog don't need a header.
            m_DialogHeader.text = "";

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        protected override void OnClickedConfirmButton()
        {
            base.OnClickedConfirmButton();
            // Disconnect form the server.
            NetworkManager.Instance.DisconnectFormServer();

            // Quit the game.
            Application.Quit();
        }

        protected override void OnClickedDenyButton()
        {
            base.OnClickedDenyButton();

            UIManager.Instance.HideUI(this);
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_DialogContext.GetComponent<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Dlog_ExitGameContext, language);

            m_ConfirmButton.GetComponentInChildren<Text>().text = 
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Dlog_ExitGameConfirmButton, language);

            m_DenyButton.GetComponentInChildren<Text>().text =
                GameLanguageManager.GetText(GameLanguageManager.KeyWord.Dlog_ExitGameDenyButton, language);
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BoxHound.UI {
    public class InGameMessageUI : UIBase {

        #region Public variables
        // -------------- Public variable -------------
        [SerializeField]
        private Text m_MessageContent;
        #endregion

        #region Private variables
        // -------------- Private variable ------------
        private string m_Message = "";
        private float m_MessageDisplayDuration = 2.0f;
        private enum MessageType
        {
            Join,
            Left,
            Kill
        }

        private string m_KillMessage = "";
        private string m_LeftMessage = "";
        private string m_JoinMessage = "";

        private string m_Middel = "";

        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.InGameMessageUI, 
                UIframework.UIManager.DisplayUIMode.Normal, 
                UIframework.UIManager.UITypes.AboveBlurEffect, 
                true, true);

            m_Message = "";
            m_MessageDisplayDuration = 4.0f;

            m_CanvasGroup.alpha = 0.0f;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;

            SetLanguage(GameLanguageManager.CurrentLanguage);
        }

        public void OnPlayerJoin(PhotonPlayer newPlayer) {
            UpdateMessage(MessageType.Join, newPlayer.name + " " + m_JoinMessage);
        }

        public void OnPlayerLeft(PhotonPlayer otherPlayer)
        {
            UpdateMessage(MessageType.Join, otherPlayer.name + " " + m_LeftMessage);
        }

        public void KillMessage(string killer, string victim)
        {
            if (PhotonNetwork.player.name.CompareTo(killer) == 0)
            {
                UpdateMessage(MessageType.Kill, " <color=yellow>" + killer + "</color> " + m_Middel + " " + victim + " " +
                    m_KillMessage);
            }
            else if (PhotonNetwork.player.name.CompareTo(victim) == 0)
            {
                UpdateMessage(MessageType.Kill, " " + killer + " " + m_Middel + " <color=yellow>" + victim + "</color> " +
                    m_KillMessage);
            }
            else
            {
                UpdateMessage(MessageType.Kill, " " + killer + " " + m_Middel + " " + victim + m_KillMessage);
            }
        }

        private void UpdateMessage(MessageType type, string message)
        {
            switch (type)
            {
                case MessageType.Kill:
                    m_Message += "\n[<color=red>" + type.ToString() + "</color>]" + message;
                    break;
                case MessageType.Join:
                    m_Message += "\n[<color=blue>" + type.ToString() + "</color>]" + message;
                    break;
                case MessageType.Left:
                    m_Message += "\n[<color=blue>" + type.ToString() + "</color>]" + message;
                    break;
                default:
#if UNITY_EDITOR
                    Debug.Log("Something is not right...");
#endif
                    break;
            }
            m_MessageContent.text = m_Message;

            m_CanvasGroup.alpha = 1.0f;
            // Stop fading out.
            StopAllCoroutines();
            // Restart the indicator fade out process.
            StartCoroutine(Fadeout());
        }

        /// <summary>
        /// Countdown the timer and when time's up, fadeout the message box.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Fadeout()
        {
            // Wait until time's up.
            yield return new WaitForSeconds(m_MessageDisplayDuration);
            // While the indicator is still visible.
            while (m_CanvasGroup.alpha > 0)
            {
                m_CanvasGroup.alpha -= 0.1f;
                yield return null;
            }
        }

        public override void EventRegister(bool reigist)
        {
            base.EventRegister(reigist);

            if (reigist) {
                MessageBroadCastManager.PlayerJoinRoomEvent += OnPlayerJoin;
                MessageBroadCastManager.PlayerLeftRoomEvent += OnPlayerLeft;
            } else {
                MessageBroadCastManager.PlayerJoinRoomEvent -= OnPlayerJoin;
                MessageBroadCastManager.PlayerLeftRoomEvent -= OnPlayerLeft;
            }
        }


        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            m_KillMessage = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Igms_KillMessage, language);
            m_LeftMessage = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Igms_LeftMessage, language);
            m_JoinMessage = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Igms_JoinMessage, language);

            m_Middel = GameLanguageManager.GetText(GameLanguageManager.KeyWord.Igms_Middel, language);
        }
    }

}

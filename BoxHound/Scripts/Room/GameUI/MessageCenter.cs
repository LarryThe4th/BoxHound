using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;

namespace BoxHound.UI
{
    public class MessageCenter : RoomUIBase
    {
        #region Public variables
        // -------------- Public variable -------------
        public Text MessageContent;
        #endregion

        #region Private variables
        // -------------- Private variable ------------
        private string m_Message = "";
        private float m_MessageDisplayDuration = 2.0f;
        private enum MessageType {
            Join,
            Left,
            Kill
        }

        private readonly string[] m_KillLogVariation = { "を仕留めた！", "をやっつけた！", "を打ち取った！" };
        #endregion

        public void Init() {
            m_Message = "";
            m_MessageDisplayDuration = 2.0f;
            DisplayUI(false);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            base.OnPhotonPlayerConnected(newPlayer);

            UpdateMessage(MessageType.Join, newPlayer.name + " は入室しました");
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);

            UpdateMessage(MessageType.Left, otherPlayer.name + " は退室しました");
        }

        public void KillMessage(string killer, string victim) {
            if (PhotonNetwork.player.name.CompareTo(killer) == 0)
            {
                UpdateMessage(MessageType.Kill, " <color=yellow>" + killer + "</color> は " + victim + " " +
                    m_KillLogVariation[Random.Range(0, m_KillLogVariation.Length - 1)]);
            }
            else if (PhotonNetwork.player.name.CompareTo(victim) == 0) {
                UpdateMessage(MessageType.Kill, " " + killer + " は <color=yellow>" + victim + "</color> " + 
                    m_KillLogVariation[Random.Range(0, m_KillLogVariation.Length - 1)]);
            }
            else {
                UpdateMessage(MessageType.Kill, " " + killer + " は " + victim +
                    m_KillLogVariation[Random.Range(0, m_KillLogVariation.Length - 1)]);
            }
        }

        private void UpdateMessage(MessageType type, string message) {
            switch (type) {
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
            MessageContent.text = m_Message;

            DisplayUI(true);
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

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            return GameRoomUI.RoomUITypes.MessageCenter;
        }
    }
}

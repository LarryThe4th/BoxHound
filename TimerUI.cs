using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    public class TimerUI : MonoBehaviour
    {
        #region Public variable
        // -------------- Public variable -------------
        public Color CountDownTimerTextColor;
        #endregion

        #region Private variable
        // -------------- Private variable -------------
        [SerializeField]
        public Text TimerText;
        [SerializeField]
        private Animation m_Animation;
        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        #endregion

        public void Init() {
            if (TimerText)
                TimerText.color = Color.white;

            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;

            m_Animation = GetComponent<Animation>();
        }


        public void ShowTimer(bool show) {
            m_CanvasGroup.alpha = show ? 1.0f : 0.0f;
        }

        public void UdpateTimer() {
            if (GameRoomManager.CurrentGameMode == null) return;

            double timeLeft = GameRoomManager.CurrentGameMode.GetRoundTimeLeft();
            int minutesLeft = Mathf.FloorToInt((float)timeLeft / 60);
            int secondsLeft = Mathf.FloorToInt((float)timeLeft) % 60;

            if (minutesLeft <= 0) {
                if (secondsLeft <= 0)
                {
                    TimerText.text = GameLanguageManager.TimesUp;
                    return;
                }

                if (secondsLeft <= 30)
                {
                    m_Animation.Play();
                    TimerText.color = CountDownTimerTextColor;
                }
            }

            TimerText.text = minutesLeft.ToString() + ":" + secondsLeft.ToString("00");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI {
    public class LeaderBoradElement : MonoBehaviour
    {
        #region Private variable
        // -------------- Private variable ------------------
        [SerializeField]
        private Text m_Index;
        [SerializeField]
        private Text m_PlayerName;
        [SerializeField]
        private Text m_Score;

        public Color localPlayerTextColor;
        #endregion

        public void ShowNode(bool enable)
        {
            this.gameObject.SetActive(enable);
        }

        public void UpdateElement(bool isLocalPlayer, string index, string playerName, int score)
        {
            m_Index.color = isLocalPlayer ? localPlayerTextColor : Color.white;
            m_PlayerName.color = isLocalPlayer ? localPlayerTextColor : Color.white;
            m_Score.color = isLocalPlayer ? localPlayerTextColor : Color.white;

            m_Index.text = index;
            m_PlayerName.text = playerName;
            m_Score.text = score.ToString();
        }

    }

}

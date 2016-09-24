using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class PlayerListNode : MonoBehaviour
    {
        #region Public varibales
        public Text Index;
        public Text PlayerName;
        public Text Score;

        public Color32 localPlayerTextColor;
        #endregion

        public void DisplayNode(bool enable) {
            this.gameObject.SetActive(enable);
        }

        public void SetInfo(bool isLocalPlayer, string index, string playerName, string score) {
            if (isLocalPlayer)
            {
                Index.color = localPlayerTextColor;
                PlayerName.color = localPlayerTextColor;
                Score.color = localPlayerTextColor;
            }
            else {
                Index.color = Color.white;
                PlayerName.color = Color.white;
                Score.color = Color.white;
            }

            Index.text = index;
            PlayerName.text = playerName;
            Score.text = score;
        }
    }
}

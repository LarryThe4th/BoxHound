using UnityEngine;
using System.Collections.Generic;

namespace BoxHound.UI
{
    // TODO: This need a better solution then this.
    public class SortByScore : IComparer<PhotonPlayer>
    {
        public int Compare(PhotonPlayer x, PhotonPlayer y)
        {
            int result = ((int)y.GetScore()).CompareTo((int)x.GetScore());
            if (result == 0)
            {
                return x.name.CompareTo(y.name);
            }
            return result;
        }
    }

    public class LeaderBoradSingleList : MonoBehaviour
    {
        #region Private variables
        // -------------- Private variable -------------
        private List<LeaderBoradElement> m_ElementList = new List<LeaderBoradElement>();

        private List<PhotonPlayer> m_PlayerList = new List<PhotonPlayer>();

        private IComparer<PhotonPlayer> ComparerScore = new SortByScore();
        #endregion

        public void Init() {
            m_ElementList = new List<LeaderBoradElement>();
            m_PlayerList = new List<PhotonPlayer>();

            foreach (var item in GetComponentsInChildren<LeaderBoradElement>())
            {
                m_ElementList.Add(item);
                Debug.Log("Here");
            }
        }

        public void UpdateList(int team = 0)
        {
            // Load player list form server.
            PhotonPlayer[] player = PhotonNetwork.playerList;

            m_PlayerList.Clear();

            for (int index = 0; index < player.Length; index++)
            {
                m_PlayerList.Add(player[index]);
            }
            m_PlayerList.Sort(ComparerScore);
            for (int index = 0; index < m_ElementList.Count; index++)
            {
                if (index < m_PlayerList.Count)
                {
                    m_ElementList[index].ShowNode(true);

                    m_ElementList[index].UpdateElement(m_PlayerList[index].isLocal,
                        (index + 1).ToString(), m_PlayerList[index].name,
                        (m_PlayerList[index].GetScore()));
                }
                else
                {
                    m_ElementList[index].ShowNode(false);
                }
            }
        }
    }
}

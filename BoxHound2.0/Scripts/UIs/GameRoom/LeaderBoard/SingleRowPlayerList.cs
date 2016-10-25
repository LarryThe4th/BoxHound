using UnityEngine;
using System.Collections.Generic;
using System;

namespace BoxHound
{
    public class SingleRowPlayerList : LeaderBoardListBase
    {
        #region Public variables
        // -------------- Public variable -------------
        public Transform ListNodeParentNode;
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        private List<PlayerListNode> m_NodeList = new List<PlayerListNode>();

        private List<PhotonPlayer> m_PlayerList = new List<PhotonPlayer>();
        #endregion

        public void Start() {
            m_NodeList = new List<PlayerListNode>();
            m_PlayerList = new List<PhotonPlayer>();
        }

        public override void Use(bool enable)
        {
            base.Use(enable);
            this.gameObject.SetActive(enable);
        }

        public override void OnPlayerConnectedToRoom(PhotonPlayer newPlayer)
        {
            UpdateList();
        }

        public override void OnPlayerDisConnectedFormRoom(PhotonPlayer player)
        {
            UpdateList();
        }

        public override void UpdateList()
        {
            // Load player list form server.
            PhotonPlayer[] player = PhotonNetwork.playerList;

            if (player.Length > m_NodeList.Count) {
                PopulateList(player.Length - m_NodeList.Count);
            }

            m_PlayerList.Clear();

            for (int index = 0; index < player.Length; index++)
            {
                m_PlayerList.Add(player[index]);
            }


            m_PlayerList.Sort(ComparerScore);


            for (int index = 0; index < m_NodeList.Count; index++)
            {
                if (index < m_PlayerList.Count) {
                    m_NodeList[index].ShowNode(true);

                    m_NodeList[index].SetInfo(m_PlayerList[index].isLocal, 
                        (index + 1).ToString(), m_PlayerList[index].name,
                        (m_PlayerList[index].GetScore()));
                } else {
                    m_NodeList[index].ShowNode(false);
                }
            }
        }

        public override void PopulateList(int nodeCount) {
            for (int index = 0; index < nodeCount; index++)
            {
                GameObject nodeObject = Instantiate(ListNodePrefab, ListNodeParentNode) as GameObject;
                PlayerListNode node = nodeObject.GetComponent<PlayerListNode>();
                m_NodeList.Add(node);
                node.ShowNode(false);
            }
        }
    }
}
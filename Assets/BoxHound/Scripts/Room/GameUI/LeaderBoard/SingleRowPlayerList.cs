using UnityEngine;
using System.Collections.Generic;
using System;

namespace Larry.BoxHound
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
        #endregion

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

            for (int index = 0; index < m_NodeList.Count; index++)
            {
                if (index < player.Length) {
                    m_NodeList[index].DisplayNode(true);
                    m_NodeList[index].SetInfo(player[index].isLocal, 
                        (index + 1).ToString(), player[index].name, 
                        player[index].GetScore().ToString());
                } else {
                    m_NodeList[index].DisplayNode(false);
                }
            }
        }

        public override void PopulateList(int nodeCount) {
            for (int index = 0; index < nodeCount; index++)
            {
                GameObject nodeObject = Instantiate(ListNodePrefab, ListNodeParentNode) as GameObject;
                PlayerListNode node = nodeObject.GetComponent<PlayerListNode>();
                m_NodeList.Add(node);
                node.DisplayNode(false);
            }
        }
    }
}
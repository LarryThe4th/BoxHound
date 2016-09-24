using UnityEngine;
using System.Collections.Generic;
using System;

namespace Larry.BoxHound
{
    public class DoubleRowPlayerList : LeaderBoardListBase
    {
        #region Public variables
        // -------------- Public variable -------------
        public Transform RedTeamListRoot;
        public Transform BlueTeamListRoot;
        #endregion

        #region Private variables
        // -------------- Private variable -------------
        private List<PlayerListNode> m_TeamRedNodeList = new List<PlayerListNode>();
        private List<PlayerListNode> m_TeamBlueNodeList = new List<PlayerListNode>();

        private List<PhotonPlayer> m_TeamRed = new List<PhotonPlayer>();
        private List<PhotonPlayer> m_TeamBlue = new List<PhotonPlayer>();
        #endregion

        public override void Use(bool enable)
        {
            base.Use(enable);
            this.gameObject.SetActive(enable);
        }

        public override void OnPlayerConnectedToRoom(PhotonPlayer newPlayer)
        {
            if (GameModeManager.GetTeam((int)newPlayer.customProperties[PlayerProperties.Team])
                    == GameModeManager.Team.Red)
                UpdateTeamList(GameModeManager.Team.Red, ref m_TeamRedNodeList, ref m_TeamRed);
            else
                UpdateTeamList(GameModeManager.Team.Blue, ref m_TeamBlueNodeList, ref m_TeamBlue);
        }

        public override void OnPlayerDisConnectedFormRoom(PhotonPlayer player)
        {
            if (GameModeManager.GetTeam((int)player.customProperties[PlayerProperties.Team])
                == GameModeManager.Team.Red)
                UpdateTeamList(GameModeManager.Team.Red, ref m_TeamRedNodeList, ref m_TeamRed);
            else
                UpdateTeamList(GameModeManager.Team.Blue, ref m_TeamBlueNodeList, ref m_TeamBlue);
        }

        public override void UpdateList()
        {
            UpdateTeamList(GameModeManager.Team.Red, ref m_TeamRedNodeList, ref m_TeamRed);
            UpdateTeamList(GameModeManager.Team.Blue, ref m_TeamBlueNodeList, ref m_TeamBlue);
        }

        private void UpdateTeamList(GameModeManager.Team team, ref List<PlayerListNode> teamNodeList, ref List<PhotonPlayer> playerList) {
            // Load player list form server.
            PhotonPlayer[] totalplayerList = PhotonNetwork.playerList;

            playerList.Clear();

            foreach (var player in totalplayerList)
            {
                if (GameModeManager.GetTeam((int)player.customProperties[PlayerProperties.Team])
                    == team) {
                    playerList.Add(player);
                }
            }


            for (int index = 0; index < teamNodeList.Count; index++)
            {
                if (index < playerList.Count)
                {
                    teamNodeList[index].DisplayNode(true);
                    teamNodeList[index].SetInfo(playerList[index].isLocal,
                        (index + 1).ToString(), playerList[index].name,
                        playerList[index].GetScore().ToString());
                }
                else
                {
                    teamNodeList[index].DisplayNode(false);
                }
            }
        }

        public override void PopulateList(int nodeCount)
        {
            for (int index = 0; index < nodeCount; index++)
            {
                if (index < (nodeCount / 2))
                {
                    GameObject nodeObject = Instantiate(ListNodePrefab, RedTeamListRoot) as GameObject;
                    PlayerListNode node = nodeObject.GetComponent<PlayerListNode>();
                    m_TeamRedNodeList.Add(node);
                    node.DisplayNode(false);
                }
                else {
                    GameObject nodeObject = Instantiate(ListNodePrefab, BlueTeamListRoot) as GameObject;
                    PlayerListNode node = nodeObject.GetComponent<PlayerListNode>();
                    m_TeamBlueNodeList.Add(node);
                    node.DisplayNode(false);
                }
            }
        }
    }
}

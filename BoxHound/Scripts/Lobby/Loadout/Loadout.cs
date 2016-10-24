using UnityEngine;
using System.Collections;
using System;

namespace BoxHound.Larry
{
    public class Loadout : LobbyUIBase
    {
        #region Public variables.
        // -------------- Public variable -------------
        #endregion

        private void Start() {
            PlayerLoadout();
        }

        private void PlayerLoadout()
        {
            PhotonNetwork.player.customProperties = new ExitGames.Client.Photon.Hashtable();
            PhotonNetwork.player.customProperties.Add(PlayerProperties.Team, 0);
            PhotonNetwork.player.customProperties.Add(PlayerProperties.Score, 0);
        }

        public override Pages GetLobbyPage()
        {
            return Pages.Loadout;
        }

        public override void OnSelectedLobbyPage(Pages selectedPage)
        {
            // If this event is call to it self.
            if (selectedPage == GetLobbyPage())
            {
                // Get the canvas group if not set yet.
                if (!m_CanvasGroup)
                    m_CanvasGroup = GetComponent<CanvasGroup>();
                m_CanvasGroup.alpha = 1;
                m_CanvasGroup.interactable = true;
                m_CanvasGroup.blocksRaycasts = true;
            }
            else {
                m_CanvasGroup.alpha = 0;
                m_CanvasGroup.interactable = false;
                m_CanvasGroup.blocksRaycasts = false;
            }
        }
    }
}

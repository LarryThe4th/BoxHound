using UnityEngine;
using System.Collections;
using System;

namespace Larry.BoxHound
{
    public class Loadout : LobbyPage
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
        }

        public override LobbyManager.Pages GetLobbyPage()
        {
            return LobbyManager.Pages.Loadout;
        }

        public override void ShowPage(bool show)
        {
            base.ShowPage(show);
        }
    }
}

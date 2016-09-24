using UnityEngine;
using System.Collections;
using System;

namespace Larry.BoxHound
{
    public class ExitLobby : LobbyPage
    {
        #region Public variables.
        // -------------- Public variable -------------
        #endregion

        public override LobbyManager.Pages GetLobbyPage()
        {
            return LobbyManager.Pages.ExitLobby;
        }

        public override void ShowPage(bool show)
        {
            base.ShowPage(show);
        }

        public void OnConfirmedExitLobby(bool exit)
        {
            if (exit)
            {
                SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
                NetWorkManager.Instance.Disconnect();
            }
            else
            {
                SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuDeny);
                LobbyManager.Instance.EnableNextPage();
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    public abstract class LobbyPage : Photon.PunBehaviour
    {
        #region Public variables.
        // -------------- Public variable -------------
        public RectTransform Content;

        #endregion

        public virtual void ShowPage(bool show) {
            if (Content)
            {
                Content.gameObject.SetActive(show);
            }
        }

        public abstract LobbyManager.Pages GetLobbyPage();
    }
}

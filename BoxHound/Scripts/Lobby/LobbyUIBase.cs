using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BoxHound.Larry
{
    [Obsolete("Use LobbyPageUIBase instead.")]
    public abstract class LobbyUIBase : Photon.PunBehaviour
    {
        #region Delget and Event
        public delegate void LobbyPageUIHandler(Pages selectedPage);
        public static event LobbyPageUIHandler LobbyPageSelectedEvent;
        public static void OnSelectedLobbyPageUI(Pages selectedPage)
        {
            if (LobbyPageSelectedEvent != null) LobbyPageSelectedEvent(selectedPage);
        }

        private void OnEnable()
        {
            EventRegister(true);
        }

        private void OnDisable()
        {
            EventRegister(false);
        }

        /// <summary>
        /// Since in unity3D the child class's OnEnable() and OnDisable()
        /// will be overwrite by parent class, use this little hack to 
        /// warp it up, so the child class and overwrite the OnEnable() 
        /// and OnDisable() method.
        /// </summary>
        /// <param name="Regist">TRUE will subscribe to the event, FALSE will unsubscribe.</param>
        protected virtual void EventRegister(bool Regist)
        {
            if (Regist)
            {
                // Subscribed to the event when this script is enabled.
                LobbyPageSelectedEvent += OnSelectedLobbyPage;
            }
            else
            {
                // Unsubscribed to the event when this script is disabled.
                LobbyPageSelectedEvent -= OnSelectedLobbyPage;
            }
        }
        #endregion

        #region Public Variables
        // -------------- Public variable -------------
        public enum Pages
        {
            RoomBrowser,
            CreateRoom,
            Loadout,
            ExitLobby,
        }
        #endregion

        #region Protected variables.
        // -------------- Protected variable -------------
        [Tooltip("The UI group control of the lobby pages.")]
        [SerializeField]
        protected CanvasGroup m_CanvasGroup;
        #endregion

        // -------------- Weird shit about Unity3D's UI --------------
        //   So, YES i do kown that directly modify the gameObject's
        // active state is not a smart choice and YES i did tried using
        // the CanvasGroup compoment and adjusting the CanvasGroup.alpha
        // to display or hide the UI conpoment.
        // 
        //   But now is the weird part:
        // 
        //   All the UI compoments are become non-interactable after 
        // adding the CanvasGroup compoment, even if i disable it at
        // the beginning of the scene or set the interactable to true,
        // all the interactable UI compoment under these CanvasGroup
        // won't react to mouse clicking anymore. Dafuq?
        // 
        //   I have no choise but roll back.
        // ---------------------------------------------------------
        //
        // ....Three hours late....
        //
        // -------------- Weird shit about Unity3D's UI #2 --------------
        // YOU HAVE TO SELECT " BLOCK RAYCASTS " TO RECEIVE RAYCASY FORM MOUSE CLICK?
        // 
        //   Seriously? Unity3d?
        //   When you say "block" means able to receive raycast, and 
        // "unblock" is to ignore raycasting?
        //   ARE YOU SERIOUS!? 
        //   YOU DONE FUCKED UUUUUP UNITY!
        // ---------------------------------------------------------

        /// <summary>
        /// Response to the page selected event.
        /// </summary>
        /// <param name="selectedPage"></param>
        public abstract void OnSelectedLobbyPage(Pages selectedPage);

        public void EnablePageFunction(bool enable) {
            m_CanvasGroup.blocksRaycasts = enable;
        }

        public abstract Pages GetLobbyPage();
    }
}

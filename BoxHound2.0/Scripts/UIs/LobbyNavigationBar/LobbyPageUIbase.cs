using UnityEngine;

namespace BoxHound.UI {
    public abstract class LobbyPageUIbase : UIBase
    {
        public abstract LobbyManager.LobbyPageCategory GetPage();

        /// <summary>
        /// Since in unity3D the child class's OnEnable() and OnDisable()
        /// will be overwrite by parent class, use this little hack to 
        /// warp it up, so the child class and overwrite the OnEnable() 
        /// and OnDisable() method.
        /// </summary>
        /// <param name="Regist">TRUE will subscribe to the event, FALSE will unsubscribe.</param>
        public override void EventRegister(bool reigist)
        {
            base.EventRegister(reigist);

            if (reigist)
            {
                // Subscribed to the event when this script is enabled.
                MessageBroadCastManager.NavigationButtonClickedEvent += PageDisplayContorl;
            }
            else
            {
                // Unsubscribed to the event when this script is disabled.
                MessageBroadCastManager.NavigationButtonClickedEvent -= PageDisplayContorl;
            }
        }

        public virtual void PageDisplayContorl(LobbyManager.LobbyPageCategory type) {
            // If clicked button's navigation category match with this button.
            if (GetPage() == type)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }
    }
}


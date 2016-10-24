using UnityEngine;
using System.Collections;

namespace BoxHound.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class RoomUIBase : Photon.PunBehaviour
    {
        #region Private variables.
        // -------------- Private variable -------------
        [Tooltip("The UI group control of the room pages.")]
        [SerializeField]
        protected CanvasGroup m_CanvasGroup;

        protected bool m_IsDisplaying = false;
        private bool m_Init = false;
        #endregion

        /// <summary>
        /// Control the UI pages display.
        /// TODO: Change this into a deleget event system.
        /// </summary>
        /// <param name="show"></param>
        public virtual void DisplayUI(bool show)
        {
            if (!m_Init) {
                m_CanvasGroup = GetComponent<CanvasGroup>();
                m_IsDisplaying = false;
                m_Init = true;
            }

            m_IsDisplaying = show;

            m_CanvasGroup.alpha = show ? 1 : 0;
            m_CanvasGroup.blocksRaycasts = show;
        }

        /// <summary>
        /// Return TURE if the UI group is displaying.
        /// </summary>
        public bool IsDisplaying() {
            return m_IsDisplaying;
        }

        public abstract GameRoomUI.RoomUITypes GetRoomUIType();
    }
}
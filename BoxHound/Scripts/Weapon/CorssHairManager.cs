using UnityEngine;

namespace BoxHound.UI
{
    public class CorssHairManager : RoomUIBase
    {
        public void Init() {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        public void ShowCorssHair(bool show) {
            m_CanvasGroup = GetComponent<CanvasGroup>();

            m_CanvasGroup.alpha = show ? 1.0f : 0.3f;
        }

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            return GameRoomUI.RoomUITypes.PlayerHUD;
        }
    }
}


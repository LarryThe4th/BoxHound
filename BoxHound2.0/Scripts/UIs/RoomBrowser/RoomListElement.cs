using UnityEngine;
using UnityEngine.UI;
using BoxHound.Manager;

namespace BoxHound.UI
{
    public class RoomListElement : MonoBehaviour
    {
        #region Events
        public void OnEnable()
        {
            MessageBroadCastManager.RoomPreviewElementClickedEvent += OnButtonVisualUpdate;
        }

        public void OnDisable()
        {
            MessageBroadCastManager.RoomPreviewElementClickedEvent -= OnButtonVisualUpdate;
        }
        #endregion

        #region Private variable
        // ---------- Private variable -----------
        [SerializeField]
        private Image m_BackgroundImage;

        [SerializeField]
        private Button m_Button;

        [SerializeField]
        private Text m_RoomIndexText;

        [SerializeField]
        private Text m_RoomNameText;

        [SerializeField]
        private Text m_PlayerCountText;

        private int m_Index = 0;
        #endregion

        public void Init()
        {
            m_BackgroundImage = GetComponent<Image>();
            m_Button = GetComponentInChildren<Button>();

#if UNITY_EDITOR
            if (!m_BackgroundImage) Debug.LogError("Background Image component is null");
            if (!m_Button) Debug.LogError("The button component is null");
            if (!m_RoomIndexText) Debug.LogError("The room index text component is null");
            if (!m_RoomNameText) Debug.LogError("The room name text component is null");
            if (!m_PlayerCountText) Debug.LogError("The player count text component is null");
#endif

            m_Button.onClick.AddListener(delegate { OnClickRoomListElementButton(); });
            m_BackgroundImage.enabled = false;
        }

        public void SetRoomListElementContext(int index, string roomName)
        {
            m_RoomIndexText.text = index.ToString();
            m_RoomNameText.text = roomName;
        }

        public void OnButtonVisualUpdate(int index, string roomName)
        {
            if (m_Index == index)
            {
                m_BackgroundImage.enabled = true;
            }
            else
            {
                m_BackgroundImage.enabled = false;
            }
        }

        public void OnClickRoomListElementButton()
        {
            MessageBroadCastManager.OnRoomPreviewElementClicked(m_Index, m_RoomNameText.text);
        }

        public void EnableDisplay(bool enable)
        {
            if (this == null) { return; }
            this.gameObject.SetActive(enable);
        }
    }
}

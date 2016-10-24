using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BoxHound.Utility;
using BoxHound.Manager;

namespace BoxHound.UI
{
    public class RoomList : MonoBehaviour
    {
        #region public Variables
        // -------------- Public variable -------------
        [Tooltip("The parent transform compoment of the room detail elements.")]
        public Transform RoomListRoot;                      // The parent transform compoment of the room list button elements.

        [Tooltip("The perfab of the room list button element.")]
        public GameObject RoomListElementPrefab;             // The perfab of the room list button element.

        [SerializeField]
        private Text m_RoomListHeaderIndex;
        [SerializeField]
        private Text m_RoomListHeaderRoomName;
        [SerializeField]
        private Text m_RoomListHeaderPlayer;
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private static List<RoomListElement> m_RoomListElementList // The list of scroll view elements of room list.
            = new List<RoomListElement>();
        private int m_RoomListElementCount = 10;
        #endregion

        public void Start()
        {
#if UNITY_EDITOR
            if (!RoomListElementPrefab) Debug.LogError("Room List Button Prefab is null");
            if (!m_RoomListHeaderIndex) Debug.LogError("Room list Header index is null");
            if (!m_RoomListHeaderRoomName) Debug.LogError("Room list Header RoomName is null");
            if (!m_RoomListHeaderPlayer) Debug.LogError("Room list Header Player is null");
#endif
        }

        private void PopulateListElement() {
            for (int index = 0; index < m_RoomListElementCount; index++)
            {
                RoomListElement element = Instantiate(RoomListElementPrefab).GetComponent<RoomListElement>();
                element.Init();
                element.EnableDisplay(false);
                element.GetComponent<RectTransform>().ResetUIWithLayoutElement(RoomListRoot);
                m_RoomListElementList.Add(element);
            }
        }

        #region Public methods.
        public void UpdateList()
        {
            // If the list is empty, that means it has not been initizalized yet.
            if (m_RoomListElementList.Count == 0) {
                PopulateListElement();
            }

            OnUpdateLanguageDisplay(GameLanguageManager.CurrentLanguage);

            // The list of information about all the available rooms in the lobby.
            RoomInfo[] roomInfoList = PhotonNetwork.GetRoomList();

            if (roomInfoList.Length == 0) {
                foreach (var item in m_RoomListElementList)
                {
                    item.EnableDisplay(false);
                }
            }
            else
            {
                for (int index = 0; index < m_RoomListElementList.Count; index++)
                {
                    // Turn off the rest of the elements.
                    if (index >= roomInfoList.Length)
                    {
                        m_RoomListElementList[index].EnableDisplay(false);
                    }
                    // Reset the others elemenet's contant.
                    else
                    {
                        m_RoomListElementList[index].EnableDisplay(true);
                        m_RoomListElementList[index].SetRoomListElementContext(index, roomInfoList[index].name);
                    }
                }
            }
        }

        public RoomListElement GetRoomDetails(int index)
        {
            if (index >= 0 && index < m_RoomListElementList.Count)
            {
                return m_RoomListElementList[index];
            }
            return null;
        }

        private void PopulateRoomListElement() {


        }

        private void OnUpdateLanguageDisplay( GameLanguageManager.SupportedLanguage language ) {
            m_RoomListHeaderIndex.text = GameLanguageManager.GetText(
                GameLanguageManager.KeyWord.Lobb_RoomListHeaderIndex, language);

            m_RoomListHeaderRoomName.text = GameLanguageManager.GetText(
                GameLanguageManager.KeyWord.Lobb_RoomListHeaderRoomName, language);

            m_RoomListHeaderPlayer.text = GameLanguageManager.GetText(
                GameLanguageManager.KeyWord.Lobb_RoomListHeaderPlayer, language);
        }
        #endregion
    }
}

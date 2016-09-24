using UnityEngine;
using System.Collections.Generic;

namespace Larry.BoxHound
{
    public class RoomList : MonoBehaviour
    {
        #region public Variables
        // -------------- Public variable -------------
        [Tooltip("The parent transform compoment of the room detail elements.")]
        public Transform RoomListParent;                        // The parent transform compoment of the room detail elements.

        [Tooltip("The perfab of the room detail element.")]
        public GameObject RoomDetailPrefab;                      // The perfab of the room detail element.
        #endregion

        #region Private Variables
        // -------------- Private variable ------------
        private List<RoomDetails> m_RoomDetailElementList                // The list of scroll view elements of room list.
            = new List<RoomDetails>();
        #endregion
         
        #region Public methods.
        public void PopulateRoomDetail()
        {
            // The list of information about all the available rooms in the lobby.
            RoomInfo[] roomInfoList = PhotonNetwork.GetRoomList();

            // In case of werid things happend like the room info list just disappeared for whatever reason.
            if (m_RoomDetailElementList == null) m_RoomDetailElementList = new List<RoomDetails>();

            // If currently the available room count is less than the room detail elemenets in the list.
            if (m_RoomDetailElementList.Count > roomInfoList.Length)
            {
                for (int index = 0; index < m_RoomDetailElementList.Count; index++)
                {
                    // Turn off the rest of the elements.
                    if (index >= roomInfoList.Length)
                    {
                        m_RoomDetailElementList[index].EnableDisplay(false);
                    }
                    // Reset the others elemenet's contant.
                    else
                    {
                        m_RoomDetailElementList[index].EnableDisplay(true);
                        m_RoomDetailElementList[index].Init(index, roomInfoList[index]);
                    }
                }
            }
            else
            {
                for (int index = 0; index < roomInfoList.Length; index++)
                {
                    // Create new room detail elements.
                    if (index > m_RoomDetailElementList.Count || m_RoomDetailElementList.Count == 0)
                    {
                        GameObject roomDetail = Instantiate(RoomDetailPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        roomDetail.transform.SetParent(RoomListParent);
                        m_RoomDetailElementList.Add(roomDetail.GetComponent<RoomDetails>());
                    }
                    m_RoomDetailElementList[index].EnableDisplay(true);
                    m_RoomDetailElementList[index].Init(index, roomInfoList[index]);
                }
            }
        }
        #endregion
    }
}

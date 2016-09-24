using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    public abstract class LeaderBoardListBase : MonoBehaviour
    {
        #region Public variables
        // -------------- Public variable -------------
        public GameObject ListNodePrefab;
        #endregion

        #region Public methods
        public virtual void Use(bool enable)
        {
            if (!NetWorkManager.IsConnected) return;
            if (enable)
                PopulateList(PhotonNetwork.room.maxPlayers);
        }

        public abstract void OnPlayerConnectedToRoom(PhotonPlayer newPlayer);

        public abstract void OnPlayerDisConnectedFormRoom(PhotonPlayer player);

        public abstract void UpdateList();

        public abstract void PopulateList(int nodeCount);
        #endregion

        #region Private methods
        private void OnEnable()
        {
            if (!NetWorkManager.IsConnected) return;
            NetWorkManager.PlayerConnected += OnPlayerConnectedToRoom;
            NetWorkManager.PlayerConnected += OnPlayerDisConnectedFormRoom;
            UpdateList();
        }

        private void OnDisable()
        {
            if (!NetWorkManager.IsConnected) return;
            NetWorkManager.PlayerConnected -= OnPlayerConnectedToRoom;
            NetWorkManager.PlayerConnected -= OnPlayerDisConnectedFormRoom;
        }
        #endregion
    }
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using BoxHound.Manager;

namespace BoxHound
{
    public class SortByScore : IComparer<PhotonPlayer>
    {
        public int Compare(PhotonPlayer x, PhotonPlayer y)
        {
            int result = ((int)y.GetScore()).CompareTo((int)x.GetScore());
            if (result == 0)
            {
                return x.name.CompareTo(y.name);
            }
            return result;
        }
    }

    public abstract class LeaderBoardListBase : MonoBehaviour
    {

        #region Public variables
        // -------------- Public variable -------------
        public GameObject ListNodePrefab;

        protected IComparer<PhotonPlayer> ComparerScore = new SortByScore();
        #endregion

        #region Public methods
        public virtual void Use(bool enable)
        {
            if (!NetworkManager.IsConnectedToServer) return;
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
            if (!NetworkManager.IsConnectedToServer) return;
            MessageBroadCastManager.PlayerJoinRoomEvent += OnPlayerConnectedToRoom;
            MessageBroadCastManager.PlayerLeftRoomEvent += OnPlayerDisConnectedFormRoom;
        }

        private void OnDisable()
        {
            if (!NetworkManager.IsConnectedToServer) return;
            MessageBroadCastManager.PlayerJoinRoomEvent -= OnPlayerConnectedToRoom;
            MessageBroadCastManager.PlayerLeftRoomEvent -= OnPlayerDisConnectedFormRoom;
        }
        #endregion
    }
}

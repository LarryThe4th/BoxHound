using UnityEngine;
using System.Collections;
using System;

namespace Larry.BoxHound
{
    public class FreeForAll : GameModeBase
    {
        #region Public variables
        [Tooltip("All the available spawn points.")]
        public Transform[] SpawnPoints = new Transform[0];
        #endregion

        public override GameModeManager.GameModes GetGameMode()
        {
            return GameModeManager.GameModes.FreeForAll;
        }

        public override void Setup(bool setEnable)
        {
            enabled = setEnable;
            if (!setEnable) return;
        }

        public override bool IsRoundFinished()
        {
            if (base.IsRoundFinished()) return true;
            return false;
        }

        public override bool IsUsingTeamRule()
        {
            return false;
        }

        /// <summary>
        /// On Initializing a new round, only the master client calls the SetRoundStartTime() method.
        /// Use that method to keep in check the when the round started.
        /// </summary>
        /// <param name="option"></param>
        public override void StartNewRound(Room option)
        {
            base.StartNewRound(option);
        }

        public override Transform GetSpawnPoint(SpawnMode mode)
        {
            // Because this is free for all, so all the spawn points are available for everyone.
            return SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)];
        }
    }
}

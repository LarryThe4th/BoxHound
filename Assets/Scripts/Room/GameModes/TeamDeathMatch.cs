using UnityEngine;
using System.Collections;
using System;

namespace Larry.BoxHound
{
    public class TeamDeathMatch : GameModeBase
    {
        #region Public variables
        [Tooltip("Spawn points for the REDs.")]
        public Transform[] RedSpawnPoints = new Transform[] { };

        [Tooltip("Spawn points for the BLUEs.")]
        public Transform[] BlueSpawnPoints = new Transform[] { };
        #endregion

        public override GameModeManager.GameModes GetGameMode()
        {
            return GameModeManager.GameModes.TeamDeathMatch;
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
            return true;
        }

        public override void StartNewRound(Room option)
        {
            base.StartNewRound(option);
        }

        public override Transform GetSpawnPoint(SpawnMode mode)
        {
            // For red side.
            if (mode == SpawnMode.RedOnly)
                return RedSpawnPoints[UnityEngine.Random.Range(0, BlueSpawnPoints.Length)];

            // For blue side
            if (mode == SpawnMode.BlueOnly)
                return BlueSpawnPoints[UnityEngine.Random.Range(0, BlueSpawnPoints.Length)];

            // If random.
            if (UnityEngine.Random.Range(0, 1) != 0)
                return BlueSpawnPoints[UnityEngine.Random.Range(0, BlueSpawnPoints.Length)];
            else
                return BlueSpawnPoints[UnityEngine.Random.Range(0, BlueSpawnPoints.Length)];
        }
    }
}

using UnityEngine;
using BoxHound.Manager;

namespace BoxHound.Larry
{
    public class FreeForAll : GameModeBase
    {
        public override GameModeManager.GameModes GetGameMode()
        {
            return GameModeManager.GameModes.FreeForAll;
        }

        /// <summary>
        /// See if the round is finished, 
        /// either by time out or reachs game goal such as one side runout of ticks.
        /// </summary>
        /// <returns></returns>
        public override bool IsRoundFinished()
        {
            // Check if time out.
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

        public override Transform GetSpawnPoint(CustomRoomOptions.Team team)
        {
            return SpawnPointsManager.Instance.GetAvailableSpwanPoint(SpawnPointsManager.GetSwpanPointMode.Random);
        }

        /// <summary>
        /// Get how much time left in the current round.
        /// </summary>
        /// <returns>Return time left in second, if error return -1 </returns>
        public override double GetRoundTimeLeft()
        {
            if (PhotonNetwork.room != null)
            {
                // If contains room property Round Start Time
                if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperties.RoundStartTime))
                {
                    RoundTimePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[RoomProperties.RoundStartTime];
                    return TotalRoundTime - RoundTimePassed;
                }
            }
            return -1;
        }
    }
}

using UnityEngine;
using BoxHound.Manager;

namespace BoxHound.Larry
{
    public class TeamDeathMatch : GameModeBase
    {
        /// <summary>
        /// Get the this game mode's game mode type.
        /// </summary>
        public override GameModeManager.GameModes GetGameMode()
        {
            return GameModeManager.GameModes.TeamDeathMatch;
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

        public override Transform GetSpawnPoint(CustomRoomOptions.Team team)
        {
            switch (team)
            {
                case CustomRoomOptions.Team.OneManArmy:
                    return SpawnPointsManager.Instance.GetAvailableSpwanPoint(SpawnPointsManager.GetSwpanPointMode.Random);
                case CustomRoomOptions.Team.Blue:
                    return SpawnPointsManager.Instance.GetAvailableSpwanPoint(SpawnPointsManager.GetSwpanPointMode.GetBlueOnly);
                case CustomRoomOptions.Team.Red:
                    return SpawnPointsManager.Instance.GetAvailableSpwanPoint(SpawnPointsManager.GetSwpanPointMode.GetRedOnly);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get how much time left in the current round.
        /// </summary>
        /// <returns>Return time left in second, if error return -1 </returns>
        public override double GetRoundTimeLeft()
        {
            if (PhotonNetwork.room != null)
            {
                // Get contains room property Round Start Time
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

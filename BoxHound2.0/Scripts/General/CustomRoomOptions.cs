using UnityEngine;
using System.Collections;

namespace BoxHound
{
    public class CustomRoomOptions : MonoBehaviour
    {
        /// <summary>
        /// Team options
        /// </summary>
        public enum Team
        {
            OneManArmy = 0,
            Red,
            Blue,
        }

        /// <summary>
        /// Available player count limit pre room.
        /// </summary>
        public static readonly byte[] AvaliablePlayerLimitOptions = new byte[] {
           4, 6, 8
        };

        /// <summary>
        /// Set the maximun health for everyplayer in this game room.
        /// </summary>
        public static readonly int[] AvaliableHealthLimitOptions = new int[] {
            100, 150, 200, 250 };

        /// <summary>
        /// Set how long can one round last.
        /// </summary>
        public static readonly int[] AvaliableTimeLimitOptions = new int[] {
            1, 2, 5, 8, 10 };

        /// <summary>
        /// Convert the team ID/index to the team enum.
        /// </summary>
        /// <param name="TeamIndex">The time index.</param>
        /// <returns></returns>
        public static Team GetTeam(int TeamIndex)
        {
            return (Team)System.Enum.GetValues(typeof(Team)).GetValue(TeamIndex);
        }
    }
}

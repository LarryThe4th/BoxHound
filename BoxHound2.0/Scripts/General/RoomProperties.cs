using UnityEngine;
using System.Collections;

namespace BoxHound
{
    /// <summary>
    /// This class the purely designed for avoiding miss spelling.
    /// These strings are use as the key value for the photon network hashtable.
    /// </summary>
    public class RoomProperties
    {
        public static readonly string TeamAScore = "AS";
        public static readonly string TeamBScore = "BS";
        public static readonly string RoundStartTime = "ST";
        public static readonly string MapIndex = "MI";
        public static readonly string RoomName = "MN";
        public static readonly string RoomCreateDate = "RD";
        public static readonly string GameModeIndex = "GM";
        public static readonly string RoundTimeLimit = "TL";
        public static readonly string HealthLimit = "HL";
        // public static readonly string RoundScoreLimit = "SL";
        // public static readonly string WinCondition = "WC";

        /// <summary>
        /// Check if there is a match key in the roon properties.
        /// </summary>
        /// <param name="room">The current game room</param>
        /// <param name="key">The key string</param>
        /// <returns>Retruns TURN if the key exist in room properties</returns>
        public static bool ContantsKey(Room room, string key) {
            if (room.customProperties.ContainsKey(key)) return true;
            Debug.LogError("Couldn't find key: " + key + " in Room named: " + room.name);
            return false;
        }
    }
}

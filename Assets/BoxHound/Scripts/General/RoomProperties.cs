using UnityEngine;
using System.Collections;

namespace Larry.BoxHound
{
    /// <summary>
    /// This class the purely designed for avoiding miss spelling.
    /// These strings are use as the key value for the photon network hashtable.
    /// </summary>
    public class RoomProperties
    {
        public static readonly string TeamAScore = "AS";
        public static readonly string TeamBScore = "BS";
        public static readonly string StartTime = "ST";
        public static readonly string MapIndex = "MI";
        public static readonly string RoomName = "MN";
        public static readonly string RoomCreateDate = "RD";
        public static readonly string GameModeIndex = "GM";
        public static readonly string TimeLimit = "TL";
        public static readonly string ScoreLimit = "SL";
        public static readonly string WinCondition = "WC";

        public static bool CheckIfRoomContantsKey(Room room, string key) {
            if (room.customProperties.ContainsKey(key)) return true;
            Debug.LogError("Couldn't find key: " + key + " in Room named: " + room.name);
            return false;
        }
    }
}

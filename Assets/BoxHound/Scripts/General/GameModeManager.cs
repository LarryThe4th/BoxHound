using UnityEngine;
using System.Collections.Generic;

namespace Larry.BoxHound
{
    public static class GameModeManager
    {
        public class GameModeDetail
        {
            public string GameModeName { get; private set; }
            public string GameModeForShort { get; private set; }
            public GameModes GameMode { get; private set; }

            public GameModeDetail(GameModes mode)
            {
                switch (mode) {
                    case GameModes.TeamDeathMatch:
                        GameModeName = "Team death match";
                        GameModeForShort = "TDM";
                        GameMode = mode;
                        break;
                    case GameModes.FreeForAll:
                        GameModeName = "Free for all";
                        GameModeForShort = "FFA";
                        GameMode = mode;
                        break;
                }
            }
        }

        private static List<GameModeDetail> m_GameModeList = new List<GameModeDetail>() {
            new GameModeDetail(GameModes.TeamDeathMatch),
            new GameModeDetail(GameModes.FreeForAll)
        };

        public enum GameModes
        {
            TeamDeathMatch,
            FreeForAll,
        }

        public enum Team {
            OneManArmy = 0,
            Red,
            Blue,
        }

        public static readonly byte[] AvaliablePlayerLimitOptions = new byte[] {
            4, 6, 8, 10
        };

        public static readonly int[] AvaliableScoreLimitOptions = new int[] {
            30, 50, 100, 150, 200 };

        public static readonly int[] AvaliableTimeLimitOptions = new int[] {
            3, 5, 8, 10, 15, 20 };

        public static readonly string[] AvaliableWinConditionOptions = new string[] {
            "First reaches score limit",  "Hightest score", "Until time's up" };

        public static int GetGameModeCount
        {
            get { return m_GameModeList.Count; }
        }

        public static GameModeDetail GetGameModeDetail(int index)
        {
            if (index >= 0 && index <= m_GameModeList.Count)
            {
                return m_GameModeList[index];
            }
            Debug.LogError("The given game mode index is out of range.");
            return null;
        }

        public static GameModeDetail GetGameModeDetail(GameModes mode) {
            foreach (var item in m_GameModeList)
            {
                if (item.GameMode == mode) return item;
            }
            return null;
        }

        public static Team GetTeam(int TeamIndex) {
            return (Team)System.Enum.GetValues(typeof(Team)).GetValue(TeamIndex);
        }
    }
}

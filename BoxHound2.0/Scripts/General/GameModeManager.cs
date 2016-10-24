using UnityEngine;
using System.Collections.Generic;

namespace BoxHound
{
    public static class GameModeManager
    {
        public class GameModeDetail
        {
            public string GameModeName { get; private set; }
            public string GameModeForShort { get; private set; }
            public GameModes GameMode { get; private set; }
            public bool UseSingleLeaderBoard { get; private set; }

            public GameModeDetail(GameModes mode)
            {
                switch (mode) {
                    case GameModes.TeamDeathMatch:
                        GameModeName = "チームデスマッチ";
                        GameModeForShort = "TDM";
                        UseSingleLeaderBoard = true;
                        break;
                    case GameModes.FreeForAll:
                        GameModeName = "フリーフォーオール";
                        GameModeForShort = "FFA";
                        UseSingleLeaderBoard = false;
                        break;
                }
                GameMode = mode;
            }
        }

        /// <summary>
        /// The list stores all the game mode information.
        /// </summary>
        private static List<GameModeDetail> m_GameModeList = new List<GameModeDetail>() {
            // new GameModeDetail(GameModes.TeamDeathMatch),
            new GameModeDetail(GameModes.FreeForAll)
        };

        /// <summary>
        /// Available game modes.
        /// </summary>
        public enum GameModes
        {
            TeamDeathMatch,
            FreeForAll,
        }

        public static string GetGameModeName(GameModes gameMode) {
            foreach (var item in m_GameModeList)
            {
                // If matches
                if (item.GameMode == gameMode) {
                    return item.GameModeName;
                }
            }
            return "";
        }

        /// <summary>
        /// Returns the number of game modes available.
        /// </summary>
        public static int GetGameModeCount
        {
            get { return m_GameModeList.Count; }
        }

        /// <summary>
        /// Get game mode detail information by index reference to the game mode.
        /// </summary>
        /// <param name="index">The game mode we are looking for.</param>
        /// <returns>Returns NULL if can't find anything.</returns>
        public static GameModeDetail GetGameModeDetail(int index)
        {
            if (index >= 0 && index <= m_GameModeList.Count)
            {
                return m_GameModeList[index];
            }
            return null;
        }

        /// <summary>
        /// Get game mode detail information by a specific game mode.
        /// </summary>
        /// <param name="mode">The game mode we are looking for.</param>
        /// <returns>Returns NULL if can't find anything.</returns>
        public static GameModeDetail GetGameModeDetail(GameModes mode) {
            foreach (var item in m_GameModeList)
            {
                if (item.GameMode == mode) return item;
            }
            return null;
        }
    }
}

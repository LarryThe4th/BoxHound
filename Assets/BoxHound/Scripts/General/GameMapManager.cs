using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Larry.BoxHound
{
    public static class GameMapManager
    {
        public class GameMap {
            public string GameMapName { get; private set; }
            public string GameMapSceneName { get; private set; }
            public Sprite GameMapPreview { get; private set; }

            public GameMap(string name, string sceneName, string mapPreviewImageName) {
                GameMapName = name;

                // I tried to use SceneManager.GetSceneByName by what ever name i pass into it, it always
                // returns a vaild scene, it seems like it's a bug since unity 5.3.
                // As for now, just make sure the scene name is correctly typed in.
                GameMapSceneName = sceneName;

                GameMapPreview = Resources.Load("MapPreview/" + mapPreviewImageName, typeof(Sprite)) as Sprite;
                if (GameMapPreview == null) { Debug.Log("Shit"); }
            }
        }

        private static List<GameMap> m_GameMapList = new List<GameMap>() {
            new GameMap("Ware House", "WareHouse", "WareHousePreview"),
            new GameMap("Facing Worlds", "WareHouse", "FacingWorldsPreview")
        };

        public static int GetMapListCount {
            get { return m_GameMapList.Count; }
        }

        /// <summary>
        /// Provide a list of all the available map list by its name.
        /// </summary>
        /// <returns>A list of all the available map's name.</returns>
        public static string[] GetAvailableMapNameList() {
            string[] list = new string[m_GameMapList.Count];
            for (int index = 0; index < list.Length; index++)
            {
                list[index] = m_GameMapList[index].GameMapName;
            }
            return list;
        }


        public static int GetGameMapID(string mapName) {
            for (int index = 0; index < m_GameMapList.Count; index++)
            {
                if (string.Compare(m_GameMapList[index].GameMapName, mapName) == 0)
                {
                    return index;
                }
            }
            return -1;
        }

        public static GameMap GetGameMap(string mapName) {
            foreach (var item in m_GameMapList)
            {
                if (string.Compare(item.GameMapName, mapName) == 0) {
                    return item;
                }
            }
            return null;
        }

        public static GameMap GetGameMap(int mapIndex) {
            if (mapIndex >= 0 && mapIndex <= m_GameMapList.Count) {
                return m_GameMapList[mapIndex];
            }
            return new GameMap(null, null, "");
        }
    }
}

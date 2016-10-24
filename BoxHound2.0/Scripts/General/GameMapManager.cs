using UnityEngine;
using System.Collections.Generic;
namespace BoxHound
{
    public static class GameMapManager
    {
        public class GameMap {
            public string GameMapName { get; set; }
            public string GameMapSceneName { get; private set; }
            public Sprite GameMapPreviewImage { get; private set; }

            public GameMap(string name, string sceneName, string mapPreviewImageName) {
                GameMapName = name;
                // I tried to use SceneManager.GetSceneByName by what ever name i pass into it, it always
                // returns a vaild scene, it seems like it's a bug since unity 5.3.
                // As for now, just make sure the scene name is correctly typed in.
                GameMapSceneName = sceneName;

                GameMapPreviewImage = Resources.Load("MapPreview/" + mapPreviewImageName, typeof(Sprite)) as Sprite;
            }
        }

        /// <summary>
        /// The list stores all the map information.
        /// </summary>
        private static List<GameMap> m_GameMapList = new List<GameMap>() {
            new GameMap("倉庫訓練所", "BoxWorld", "BoxWorldPreview"),
            new GameMap("対立世界", "Lab", "FacingWorldsPreview")
        };

        /// <summary>
        /// Get the number of maps available.
        /// </summary>
        public static int GetMapCount {
            get { return m_GameMapList.Count; }
        }

        public static string GetMapName {
            get { return ""; }
        }



        /// <summary>
        /// Provide a list of all the available map list by its name.
        /// Maybe i can just use Linq to do this...
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

        /// <summary>
        /// Get the id/index of the map in the list, if exist that is.
        /// </summary>
        /// <param name="mapName">The target map's name.</param>
        /// <returns>Returns -1 when can't find the map in the list with the given name.</returns>
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

        /// <summary>
        /// Get game map detail information by a specific name.
        /// </summary>
        /// <param name="mapName">The map we are looking for.</param>
        /// <returns>Returns NULL if can't find anything.</returns>
        public static GameMap GetGameMap(string mapName) {
            foreach (var item in m_GameMapList)
            {
                if (string.Compare(item.GameMapName, mapName) == 0) {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Get game map detail information by its id/index.
        /// </summary>
        /// <param name="mapIndex">The map we are looking for.</param>
        /// <returns>Returns NULL if can't find anything.</returns>
        public static GameMap GetGameMap(int mapIndex) {
            if (mapIndex >= 0 && mapIndex <= m_GameMapList.Count) {
                return m_GameMapList[mapIndex];
            }
            return null;
        }
    }
}

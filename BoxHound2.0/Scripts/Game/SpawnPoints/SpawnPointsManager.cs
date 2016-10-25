using UnityEngine;
using System.Collections.Generic;

namespace BoxHound.Larry
{
    public class SpawnPointsManager : Singleton<SpawnPointsManager>
    {
        public enum GetSwpanPointMode {
            GetBlueOnly,
            GetRedOnly,
            Random
        }

        #region Private Variables
        // -------------- Private variable ------------
        [Tooltip("The center of the map, when player spawn or respawn " +
            "the player object will always faceing to this location")]
        [SerializeField]
        private Transform m_MapCenter;

        [Tooltip("The spawn points parent object for the BLUEs")]
        [SerializeField]
        private Transform m_BlueSpawnPointsContainer;
        // The spawn list for the BLUEs
        private List<SpawnPoint> m_BlueSpawnPointList = new List<SpawnPoint>();

        [Tooltip("The spawn points parent object for the REDs")]
        [SerializeField]
        private Transform m_RedSpawnPointsContainer;
        // The spawn list for the REDs
        private List<SpawnPoint> m_RedSpawnPointList = new List<SpawnPoint>();
        #endregion

        void Start() {
            m_BlueSpawnPointList = new List<SpawnPoint>();
            m_RedSpawnPointList = new List<SpawnPoint>();

            foreach (var spawnPoint in m_BlueSpawnPointsContainer.GetComponentsInChildren<SpawnPoint>())
            {
                // Set the spawn point facing to the map center
                spawnPoint.transform.LookAt(new Vector3(
                    m_MapCenter.position.x, 
                    spawnPoint.transform.position.y, 
                    m_MapCenter.position.z));
                m_BlueSpawnPointList.Add(spawnPoint);
            }

            foreach (var spawnPoint in m_RedSpawnPointsContainer.GetComponentsInChildren<SpawnPoint>())
            {
                // Set the spawn point facing to the map center
                spawnPoint.transform.LookAt(new Vector3(
                    m_MapCenter.position.x,
                    spawnPoint.transform.position.y,
                    m_MapCenter.position.z));
                m_RedSpawnPointList.Add(spawnPoint);
            }
        }

        /// <summary>
        /// Get available spwan point by the specific rule.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Transform GetAvailableSpwanPoint(GetSwpanPointMode mode) {
            switch (mode) {
                // Get spwan point only form the BLUEs's list
                case GetSwpanPointMode.GetBlueOnly:
                    return GetSpawnPoint(m_BlueSpawnPointList);
                // Get spwan point only form the REDs's list
                case GetSwpanPointMode.GetRedOnly:
                    return GetSpawnPoint(m_RedSpawnPointList);
                // Get random spwan point form both list.
                case GetSwpanPointMode.Random:
                    if (UnityEngine.Random.Range(0, 1) == 1)
                        return GetSpawnPoint(m_BlueSpawnPointList);
                    else
                        return GetSpawnPoint(m_RedSpawnPointList);
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Get available spawn point form the given list 
        /// </summary>
        /// <param name="list">The target list.</param>
        /// <returns></returns>
        private Transform GetSpawnPoint(List<SpawnPoint> list) {
            List<SpawnPoint> pool = new List<SpawnPoint>();

            foreach (SpawnPoint spawnPoint in list)
            {
                if (spawnPoint.SpawnPointAvailable)
                    pool.Add(spawnPoint);
            }

            return pool[UnityEngine.Random.Range(0, pool.Count - 1)].transform;
        }
    }
}


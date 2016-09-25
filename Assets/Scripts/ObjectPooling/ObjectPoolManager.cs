using UnityEngine;
using System.Collections.Generic;

namespace Larry.BoxHound {

    public class ObjectPoolManager : MonoBehaviour
    {
        // Store all the pooling object in this dictionary.
        private Dictionary<int, Queue<ObjectInstance>> m_PoolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

        // Create a new object pool
        public void CreaterPool(PoolObject prefab, Transform parent, int poolSize) {
            // Use the prefab instance id as the dictionary key.
            int poolKey = prefab.GetInstanceID();

            // If the key is not exist in the dictionary
            if (!m_PoolDictionary.ContainsKey(poolKey)) {
                // Add a new pool
                m_PoolDictionary.Add(poolKey, new Queue<ObjectInstance>());
               for (int index = 0; index < poolSize; index++)
                {
                    // Create new object
                    GameObject gameObject = Instantiate(prefab.gameObject) as GameObject;

                    // Set its parent transform
                    gameObject.transform.SetParent(parent);

                    ObjectInstance newObject = new ObjectInstance(gameObject.GetComponent<PoolObject>());

                    // Add the object into the pool.
                    m_PoolDictionary[poolKey].Enqueue(newObject);
                }
            }
        }

        public void ReuseObject(int poolKey, Vector3 position, Quaternion rotation, object[] options) {
            // Check if key exist.
            if (m_PoolDictionary.ContainsKey(poolKey)) {
                // Get the last object in the queue
                ObjectInstance reuseObject = m_PoolDictionary[poolKey].Dequeue();
                // Resign it into the queue.
                m_PoolDictionary[poolKey].Enqueue(reuseObject);
                // Call the reuse method.
                reuseObject.Reuse(position, rotation, options);
            }
        }

        #region Singleton pattern of the Object Pool Manager
        private static ObjectPoolManager m_Instance;
        public static ObjectPoolManager Instance
        {
            get
            {
                // If the instance if null
                if (!m_Instance)
                {
                    // Try find the object in scene
                    m_Instance = FindObjectOfType<ObjectPoolManager>();
                    // If couldn't find it
                    if (!m_Instance)
                    {
                        // Create a new one
                        GameObject manager = new GameObject("ObjectPoolManager", typeof(ObjectPoolManager));
                        m_Instance = manager.GetComponent<ObjectPoolManager>();
                    }
                }
                return m_Instance;
            }
        }
        #endregion
    }

    public class ObjectInstance {
        // The instance of pooling object
        private PoolObject m_Instance;

        // The transform of the pooling game object
        private Transform m_PoolObjectTransform;

        // The constructor of the object instance 
        public ObjectInstance(PoolObject poolObjectInstance) {
            m_Instance = poolObjectInstance;
            // Initialize the new object
            m_Instance.Init();
            m_PoolObjectTransform = m_Instance.transform;
            // Hide the object
            m_Instance.EnableObject(false);
        }

        public void Reuse(Vector3 position, Quaternion rotation, object[] options) {
            // Reset the object's location
            m_PoolObjectTransform.position = position;
            m_PoolObjectTransform.rotation = rotation;

            // Show the object
            m_Instance.EnableObject(true);

            // Reuse the object
            m_Instance.OnObjectReuse(options);
        }
    }

    // All the pooling object should inherit form this base class
    public abstract class PoolObject : MonoBehaviour
    {
        public void EnableObject(bool enable) {
            gameObject.SetActive(enable);
        }

        public abstract void Init();

        public abstract void OnObjectReuse(object[] options);
    }
}
using UnityEngine;
using System.Collections;

namespace BoxHound.Larry
{
    public class Utility
    {
        /// <summary>
        /// A safety check for all the MonoBehaviour compoment cache variables based on the target's tag.
        /// </summary>
        /// <typeparam name="T">The target cache variable.</typeparam>
        /// <param name="target">The MonoBehaviour compoment cache variable itself.</param>
        /// <param name="parentGameObject">The parent gameObject of this compoment.</param>
        /// <param name="tag">The tag we are going to compare with.</param>
        /// <returns>Return true if we found the compoment under the parent gameObject.</returns>
        public static bool ApplyCacheByTag<T>(ref T target, GameObject parentGameObject, string tag) where T : Component
        {
            if (target) return true;
            foreach (T item in parentGameObject.GetComponentsInChildren<T>())
            {
                if (item.CompareTag(tag))
                {
                    target = item;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A safety check for all the MonoBehaviour compoment cache variables based on the target's gameObject name.
        /// Make sure it is a unique name under the parent gameObject or it won't sreach for multiple target but only the first one on list.
        /// </summary>
        /// <typeparam name="T">The target cache variable.</typeparam>
        /// <param name="target">The MonoBehaviour compoment cache variable itself.</param>
        /// <param name="parentGameObject">The parent gameObject of this compoment.</param>
        /// <param name="name">The name of the gameObject we are looking for.</param>
        /// <returns>Return true if we found the compoment under the parent gameObject.</returns>
        public static bool ApplyCacheByName<T>(ref T target, GameObject parentGameObject, string name) where T : Component
        {
            if (target) return true;
            foreach (T item in parentGameObject.GetComponentsInChildren<T>())
            {
                if (item.gameObject.name.CompareTo(name) == 0)
                {
                    target = item;
                    return true;
                }
            }
            return false;
        }

    }
}

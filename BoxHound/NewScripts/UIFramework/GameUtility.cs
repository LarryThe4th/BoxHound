using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System;

namespace BoxHound
{
    public class GameUtility : MonoBehaviour
    {
        /// <summary>
        /// A wrapper of the unity engine's random.range method.
        /// </summary>
        /// <param name="numberA">The minimun number of the range.</param>
        /// <param name="numberB">The maximun number of the range.</param>
        /// <returns></returns>
        public static int GetRandomInt(int numberA, int numberB)
        {
            if (numberA < numberB)
                return Random.Range(numberA, numberB);
            else
                return Random.Range(numberB, numberA);
        }

        /// <summary>
        /// Clear the menory. 
        /// Use it with caution because it takes quite a few CPU cauluation.
        /// Use if when switching scene.
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Split the given string into half or more base on the ; mark.
        /// </summary>
        /// <param name="str">The target string.</param>
        /// <returns></returns>
        public static string[] DivsionString(string str)
        {
            return str.Split(';');
        }

        /// <summary>
        /// Check if there is a key that is aleady in the player prefabs.
        /// </summary>
        /// <param name="key">The key value</param>
        /// <returns>Reutns TRUE if the key did exist.</returns>
        public static bool PlayerPerfabHasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.GetInt() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static int GetIntFormPlayerPerf(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.GetFloat() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static float GetFloatFormPlayerPerf(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.GetInt() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static string GetstringFormPlayerPerf(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        /// <summary>
        /// A wapper methd of the PlayerPrefs.SetFloat() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static void SetIntToPlayerPerf(string key, int valueInt)
        {
            PlayerPrefs.SetInt(key, valueInt);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.SetFloat() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static void SetIntToPlayerPerf(string key, float valueFloat)
        {
            PlayerPrefs.SetFloat(key, valueFloat);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.SetString() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static void SetStringToPayerPerf(string key, string valueString)
        {
            PlayerPrefs.SetString(key, valueString);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.DeleteAll() method.
        /// </summary>
        public static void DeleteAllKey()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.DeleteKey() method.
        /// </summary>
        /// <param name="key">The target value's key.</param>
        public static void DeleteTheKey(string key) {
            PlayerPrefs.DeleteKey(key);
        }

        /// <summary>
        /// Search all child gameObject under the given parent gameObject, if the name
        /// matchs the target child name, return its transform.
        /// </summary>
        /// <param name="parent">The parent gameObject.</param>
        /// <param name="childName">The name of the target child transform.</param>
        /// <returns>Return TRUE if match.</returns>
        public static Transform FindChildTransform(GameObject parent, string childName) {
            Transform searchResult = parent.transform.Find(childName);
            if (searchResult == null)
            {
                foreach (Transform item in parent.transform)
                {
                    searchResult = FindChildTransform(item.gameObject, childName);
                    if (searchResult != null) {
                        return searchResult;
                    }
                }
            }
            return searchResult;
        }

        /// <summary>
        /// Search all child transform for its component under the parent gameObject and
        /// see if any transform's name matchs the given child name.
        /// </summary>
        /// <typeparam name="T">The target component.</typeparam>
        /// <param name="parent">The parent gameObject.</param>
        /// <param name="childName">The target child's name.</param>
        /// <returns></returns>
        public static T GetChildComponent<T>(GameObject parent, string childName) where T : Component {
            Transform searchResult = FindChildTransform(parent, childName);
            if (searchResult != null)
                return searchResult.gameObject.GetComponent<T>();
            return null;
        }

        /// <summary>
        /// Add a component to the target child gameObject, if the target gameObject
        /// already has that conponent(s), destory them then add a new one.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="parent">The parent gameObject.</param>
        /// <param name="childName">The name of the target child gameObject.</param>
        /// <returns>Returns the component if the target child gameObject exist.</returns>
        public static T AddComponentToChild<T>(GameObject parent, string childName) where T : Component {
            Transform searchResult = FindChildTransform(parent, childName);
            if (searchResult != null) {
                foreach (T item in searchResult.GetComponents<T>())
                {
                    Destroy(item);
                }
                return searchResult.gameObject.AddComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// Add child gameObject to the target parent gameObject.
        /// Once added, reset the child gameObject's loacl transform and set its layer matchs the parent.
        /// </summary>
        /// <param name="parent">The parent gameObject.</param>
        /// <param name="child">The child gameObject.</param>
        public static void AddChildToParent(Transform parent, Transform child) {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localPosition = Vector3.one;
            child.localEulerAngles = Vector3.zero;
            parent.gameObject.SetLayer(parent.gameObject.layer);
        }
    }

    /// <summary>
    /// Set target gameObject's layer under the parent gameObject.
    /// </summary>
    public static class GameObjectExtension
    {
        public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
        {
            parent.layer = layer;
            if (includeChildren)
            {
                foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = layer;
                }
            }
        }
    }

    public static class RectTransformExtensions
    {
        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }
        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 pivot, Vector2 minAnchor, Vector2 maxAnchor)
        {
            trans.pivot = pivot;
            trans.anchorMin = minAnchor;
            trans.anchorMax = maxAnchor;
        }

        public static void ExpandToMaxFormCenter(this RectTransform trans) {
            SetPivotAndAnchors(trans, new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(1.0f, 1.0f));
            SetDefaultScale(trans);
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;
        }

        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }
        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }
        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }
        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }
        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }
    }
}
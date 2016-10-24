using UnityEngine;
using System;

namespace BoxHound.Utility { 
    public class GameUtility : MonoBehaviour {
        /// <summary>
        /// Add child gameObject to the target parent gameObject.
        /// Once added, reset the child gameObject's loacl transform and set its layer matchs the parent.
        /// </summary>
        /// <param name="parent">The parent gameObject.</param>
        /// <param name="child">The child gameObject.</param>
        public static void AddChildToParent(Transform parent, Transform child)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localPosition = Vector3.one;
            child.localEulerAngles = Vector3.zero;
            parent.gameObject.SetLayer(parent.gameObject.layer);
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

    /// <summary>
    /// A extension for the rectTransform
    /// </summary>
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

        public static void ResetUIWithLayoutElement(this RectTransform trans, Transform parent) {
            trans.SetParent(parent);
            trans.localScale = Vector3.one;
            trans.localPosition = new Vector3(
                trans.localPosition.x,
                trans.localPosition.y,
                0
            );
        }

        public static void ExpandToMaxFormCenter(this RectTransform trans)
        {
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

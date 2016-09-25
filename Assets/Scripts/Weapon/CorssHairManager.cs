using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Larry.BoxHound
{
    public class CorssHairManager : MonoBehaviour
    {
        #region Private variables
        // -------------- Private variable ------------
        [SerializeField]
        private Image CorssHairImage;
        #endregion

        public void Start() {
            if (!CorssHairImage)
                CorssHairImage = GetComponentInChildren<Image>();
            if (!CorssHairImage) Debug.Log("Couldn't find corss hair image.");

            ShowCorssHair(false);
        }

        public void ShowCorssHair(bool show) {
            if (CorssHairImage)
                CorssHairImage.enabled = show;
        }
    }
}


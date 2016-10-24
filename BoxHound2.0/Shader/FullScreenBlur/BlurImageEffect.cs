using UnityEngine;

namespace BoxHound
{
    [ExecuteInEditMode]
    public class BlurImageEffect : MonoBehaviour
    {
        public Material EffectMaterial;
        
        private static bool UseEffect = false;
        [Range(0, 10)]
        [Tooltip("Set how many times will the source texture sampling perform.")]
        [SerializeField]
        private int Samples;
        [Range(0, 6)]
        [Tooltip("Lower the resolution of the camera rendered texture")]
        [SerializeField]
        private int DownRes;

        #region Events
        private void OnEnable()
        {
            // Subscribe the function to the event
            MessageBroadCastManager.EnableCameraBlurEffectEvent += EnableBlurEffect;
        }

        private void OnDisable()
        {
            // Unsubscribe the function form the event
            MessageBroadCastManager.EnableCameraBlurEffectEvent -= EnableBlurEffect;
        }

        private void EnableBlurEffect(bool enable) {
            UseEffect = enable;
        }
        #endregion

        void OnRenderImage(RenderTexture source, RenderTexture finalResult) {
            if (!UseEffect) return;

            // Downgrade the source texture's resolution so that the original sample
            // is already a little bit "blur" on the bigger size screen
            // This can effectively reduce the needs of times of sampling the texture,
            // and even get a better result.
            int width = source.width >> DownRes;
            int height = source.height >> DownRes;

            // Create a temporary texture
            RenderTexture baseTexture = RenderTexture.GetTemporary(width, height);
            // Cache the texture form the rendering camera as the base sample
            Graphics.Blit(source, baseTexture);

            for (int index = 0; index < Samples; index++)
            {
                // Create a other temporary texture
                RenderTexture temporaryTexture = RenderTexture.GetTemporary(width, height);
                // Mix the base texture with the effect materail and cache into the new created texture.
                Graphics.Blit(baseTexture, temporaryTexture, EffectMaterial);
                // Now the baseTexture and temporaryTexture are the same thing
                // Release the baseTexture
                RenderTexture.ReleaseTemporary(baseTexture);
                // Apply the temporaryTexture to the baseTexture
                baseTexture = temporaryTexture;
            }

            // After a couple of times of looping, we apply the base texture to the final result
            Graphics.Blit(baseTexture, finalResult);
            // Release the baseTexture
            RenderTexture.ReleaseTemporary(baseTexture);
        }
    }
}

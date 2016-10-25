using UnityEngine;

namespace BoxHound
{
    /// <summary>
    /// A staic helper class make life much easier.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Gets and sets the player score using the custom helper class 
        /// GetCustomPlayerProperty and SetCustomPlayerProperty.
        /// </summary>
        public static void AddScoreCount(PhotonView view, int addScore)
        {
            int current  = GetCustomPlayerProperty<int>(view, PlayerProperties.Score, 0);

            SetCustomPlayerProperty<int>(view, PlayerProperties.Score, (current + addScore));
        }




        /// <summary>
        ///  Get the component in the same object.
        /// </summary>
        /// <typeparam name="T">The type of the compoment you r looking for.</typeparam>
        /// <param name="gameObject">Where this compoment should be.</param>
        /// <param name="targetComponent">The target compoment</param>
        /// <returns></returns>
        public static T GetCachedComponent<T>(GameObject gameObject, ref T targetComponent) where T : MonoBehaviour
        {

            if (targetComponent == null)
            {
                targetComponent = gameObject.GetComponent<T>();
            }

            return targetComponent;
        }

        /// <summary>
        /// Get the custom player property
        /// </summary>
        /// <typeparam name="T">The type of the data you wanna get. Such as int, float, etc..</typeparam>
        /// <param name="view">The photonview.</param>
        /// <param name="property">The property key.</param>
        /// <param name="defaultValue">A default value in case we can't find the player property.</param>
        /// <returns>Return the value form custom player property.</returns>
        public static T GetCustomPlayerProperty<T>(PhotonView view, string property, T defaultValue)
        {
            if (!NetworkManager.IsConnectedToServer) return defaultValue;

            // Check if the player property is already exist
            if (view != null &&
                view.owner != null &&
                view.owner.customProperties.ContainsKey(property))
            {
                return (T)view.owner.customProperties[property];
            }

            // If not, return the default value.
            return defaultValue;
        }

        /// <summary>
        /// Set the custom player property
        /// </summary>
        /// <typeparam name="T">The type of the data you wanna set. Such as int, float, etc..</typeparam>
        /// <param name="view">The photonview.</param>
        /// <param name="property">The property key.</param>
        /// <param name="value">The value you wanna set.</param>
        public static void SetCustomPlayerProperty<T>(PhotonView view, string property, T value)
        {
            if (!NetworkManager.IsConnectedToServer) return;

            // Create a new Hashtable that is provide by photon network to 
            // ensure that the data can be synchronized between all clients
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add(property, value);

            // Use the SetCustomProperties function to set new values and update existing ones
            // This function saves the data locally and sends synchronize operations so that every
            // client receives the update as well. 
            // Don't set PhotonView.owner.customProperties directly as it wouldn't be synchronized

            // This is a function specific for UPDATE (tho it says "Set") online stored value.
            // It saves the data online and sends synchronize operations so that every client 
            // receives the update as well.
            // Notice that the differents between "PhotonView.owner.customProperties" and "PhotonView.owner.SetCustomProperties"
            // "PhotonView.owner.customProperties" Only creates custom properties but not update it.
            // If call "PhotonView.owner.customProperties" it will overwrite the data and wouldn't be synchronized.
            if (view.owner.customProperties.ContainsKey(property))
                view.owner.SetCustomProperties(properties);
            else
                Debug.Log("Can't found such property: " + property);
        }
    }
}


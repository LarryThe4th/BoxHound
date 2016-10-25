using UnityEngine;
using System.Collections.Generic;

namespace BoxHound.Manager
{
    public static class HelperInformationManger
    {
        public enum Category
        {
            None = 1,
            UserNickName,
            NoAvailableRoom,

            RoomOptionUserNickName,
            RoomOptionHealthLimit,
            RoomOptionGameMode,
            RoomOptionRoomName,
            InGameHelp,
        }

        #region Private variable
        private static SaveAndLoad<HelperInfoXMLFormat> m_SaveAndLoad = new SaveAndLoad<HelperInfoXMLFormat>();

        private static Dictionary<string, HelperInfoXMLFormat> m_InformationLibrary = new Dictionary<string, HelperInfoXMLFormat>();
        private static List<Category> m_FastLookup = new List<Category>();
        #endregion

        /// <summary>
        /// Get the helper information by the category.
        /// </summary>
        /// <param name="category">The category of the helper information.</param>
        /// <returns></returns>
        public static HelperInfoXMLFormat GetHelperInfo(string category) {
            HelperInfoXMLFormat info = null;
            m_InformationLibrary.TryGetValue(category, out info);
            return info;
        }

        /// <summary>
        /// Get the helper information by the index.
        /// </summary>
        /// <param name="index">The index in the fast loop up list</param>
        /// <returns>Return null if the index is out of range or can't find such info in the library.</returns>
        public static HelperInfoXMLFormat GetHelperInfo(int index) {
            if (index >= 0 && index < m_FastLookup.Count) {
                return GetHelperInfo(m_FastLookup[index].ToString());
            }
            return null;
        }

        /// <summary>
        /// Get the number of helper information in the library.
        /// Return -1 if the library is null. 
        /// </summary>
        public static int GetHelperCount {
            get { if (m_InformationLibrary != null) return m_InformationLibrary.Count;
                return -1;
            }
        }

        public static void Init() {
            m_SaveAndLoad = new SaveAndLoad<HelperInfoXMLFormat>();

            if (m_InformationLibrary == null)
                m_InformationLibrary = new Dictionary<string, HelperInfoXMLFormat>();
            else m_InformationLibrary.Clear();

            if (m_FastLookup == null)
                m_FastLookup = new List<Category>();
            else m_FastLookup.Clear();

            string HelperFilePath = "HelperInfo/HelperInfo_JP";
            // Load the helper information when the first time this manager been called.
            foreach (var data in m_SaveAndLoad.LoadInitData(HelperFilePath).DataList)
            {
                m_InformationLibrary.Add(data.InformationCategory.ToString(), data);
                m_FastLookup.Add(data.InformationCategory);
            }
        }

        // Static class constructor.
        static HelperInformationManger()
        {
            Init();
        }
    }
}

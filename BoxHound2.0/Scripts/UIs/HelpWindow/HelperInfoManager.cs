using UnityEngine;
using System;
using System.Collections.Generic;

namespace BoxHound {
    
    public static class HelperInfoManager
    {
        public enum HelpInfoCategory {
            MainMenu,
            RoomBrowser,
            CreateRoom,
            InGame,
            CharacterControl
        }

        public enum KeyWord
        {
            Main_NickName,
            Main_LoginButton
        }

        public enum InfoType {
            Header,
            Info,
        }

        private static readonly string m_HelpInfoFileName = "HelpInfo";
        private static readonly string m_HelpInfoCategoryFileName = "HelpInfoCategory";
        private static ExeclDataManager m_InfoManager = new ExeclDataManager();
        private static ExeclDataManager m_CategoryManager = new ExeclDataManager();

        // TODO: move this into CSV file.
        private static Dictionary<HelpInfoCategory, KeyWord[]> m_HelpInfoDictionary = new Dictionary<HelpInfoCategory, KeyWord[]>() {
            { HelpInfoCategory.MainMenu, new KeyWord[] { KeyWord.Main_LoginButton, KeyWord.Main_NickName } },
        };

        public static KeyWord[] GetHelperInfoCategoryContent(HelpInfoCategory category) {
            if (m_HelpInfoDictionary == null) return null;
            if (m_HelpInfoDictionary.ContainsKey(category)) {
                return m_HelpInfoDictionary[category];
            }
            return new KeyWord[0];
        }

        public static string GetCategroyText(HelpInfoCategory category) {
            return m_CategoryManager.LoadDataFormFile(
                GameLanguageManager.CurrentLanguage.ToString(), category.ToString());
        }

        public static string GetHeaderText(KeyWord keyword)
        {
            return m_InfoManager.LoadDataFormFile(
                GameLanguageManager.CurrentLanguage.ToString(), keyword.ToString() + InfoType.Header.ToString());
        }

        public static string GetInfoText(KeyWord keyword)
        {
            return m_InfoManager.LoadDataFormFile(
                GameLanguageManager.CurrentLanguage.ToString(), keyword.ToString() + InfoType.Info.ToString());
        }


        static HelperInfoManager() {
            m_InfoManager.LoadFile(m_HelpInfoFileName);
            m_CategoryManager.LoadFile(m_HelpInfoCategoryFileName);
        }
    }
}

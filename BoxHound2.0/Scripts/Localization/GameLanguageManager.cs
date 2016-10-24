using UnityEngine;
using System.Collections;

namespace BoxHound {
    public static class GameLanguageManager
    {
        public enum SupportedLanguage
        {
            EN = 0, // English
            JP,     // Japanese
            CN,     // Chinese
        }

        public enum KeyWord {
            TimeUnit = 0,
            TimeLeft,
            TimesUp,
            Main_NickNameInputFieldTip,
            Main_LoginButtonText,
            Menu_SettingsButtonText,
            Menu_HelpButtonText,
            Menu_AboutButtonText,
            Menu_ReturnButtonText,
            Menu_ExitRoomButtonText,
            Help_ReturnButtonText,
            Help_WindowHeaderText,
            Help_WindowCategoryText,
            Sett_SettingsWindowHeader,
            Sett_LanguageCategoryHeader,
            Sett_LanguageCategory,
            Sett_SettingsWindowSaveButton,
            Sett_SettingsWindowReturnButton,
            Dlog_ExitGameContext,
            Dlog_ExitGameConfirmButton,
            Dlog_ExitGameDenyButton,
            Dlog_NoRoomAvailableText,
            Lobb_RoomListHeaderIndex,
            Lobb_RoomListHeaderRoomName,
            Lobb_RoomListHeaderPlayer,
            NavB_RoomBrowserButtonText,
            NavB_CreateRoomButtonText,
            NavB_LoadOutButtonText,
            CreR_RoomNameInputLable,
            CreR_MapSelectLable,
            CreR_PlayerLimitLable,
            CreR_GameModeLable,
            CreR_TimeLimiLable,
            CreR_HealthLimitLabel,
            CreR_CreatingRoomText,
            CreR_NeedAName,
            CreR_SameNameExistWarning,
            CreR_CreateRoomConfirm,
            CreR_CreateRoomDeny,
            RooP_PlayerCountLabel,
            RooP_HealthLimitLabel,
            RooP_TimeLimitLabel,
            RooP_JoinButtonRoomFull,
            RooP_JoinButtonCanJoin,
            Roop_ScoreLimitLabel,
            Open_StartGameTextPleaseStandBy,
            Open_StartGameTextStart,
            Weap_ReloadingText,
            Weap_OutOfAmmo,
            Resp_RespawningInSce,
            Resp_MurderWeaponText,
            Igms_KillMessage,
            Igms_LeftMessage,
            Igms_JoinMessage,
            Igms_Middel,
        }

        public static string TimeUnit {
            get; private set;
        }

        public static string TimeLeft {
            get; private set;
        }

        public static string TimesUp
        {
            get; private set;
        }

        public static SupportedLanguage CurrentLanguage {
            get; private set;
        }

        public static void ChangeLanguage(SupportedLanguage language) {
            CurrentLanguage = language;
            MessageBroadCastManager.OnGameLanguageChange(CurrentLanguage);
            GeneralSettings();
        }

        private static void GeneralSettings() {
            TimeUnit = GetText(KeyWord.TimeUnit, CurrentLanguage);
            TimeLeft = GetText(KeyWord.TimeLeft, CurrentLanguage);
            TimesUp = GetText(KeyWord.TimesUp, CurrentLanguage);
        }

        private static readonly string m_LanguageConfigurationFileName = "LanguageCfg";
        private static ExeclDataManager m_Manager = new ExeclDataManager();

        public static string GetText(KeyWord keyword, SupportedLanguage language) {
            return m_Manager.LoadDataFormFile(language.ToString(), keyword.ToString());
        }

        static GameLanguageManager() {
            CurrentLanguage = SupportedLanguage.JP;
            m_Manager.LoadFile(m_LanguageConfigurationFileName);
            GeneralSettings();
        }
    }
}

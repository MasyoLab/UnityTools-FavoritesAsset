﻿
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    [System.Serializable]
    class SettingData {
        public LanguageEnum Language = LanguageEnum.English;
        public string ImportTarget = string.Empty;
        public string ExportTarget = string.Empty;
    }
}
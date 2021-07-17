using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class SettingManager {
        /// <summary>
        /// 保存リスト
        /// </summary>
        SettingData _ref = null;
        SettingData _linker {
            get {
                if (_ref == null) {
                    _ref = LoadSettingData();
                }
                return _ref;
            }
        }

        public SettingData Data => _linker;

        public LanguageEnum Language {
            get => _linker.Language;
            set {
                if (_linker.Language != value) {
                    _linker.Language = value;
                    SaveSettingData();
                }
            }
        }

        public void SaveSettingData() {
            SaveLoad.Save(JsonUtility.ToJson(_linker), SaveLoad.GetSaveDataPath(CONST.SETTING_DATA));
        }

        SettingData LoadSettingData() {
            string jsonData = SaveLoad.Load(SaveLoad.GetSaveDataPath(CONST.SETTING_DATA));

            // json から読み込む
            var assets = JsonUtility.FromJson<SettingData>(jsonData);
            if (assets == null) {
                return new SettingData();
            }
            return assets;
        }
    }
}
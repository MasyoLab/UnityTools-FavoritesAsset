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
                    _ref = LoadAssetInfo();
                }
                return _ref;
            }
        }

        public SettingData Data => _linker;

        public LanguageEnum Language {
            get => _linker.Language;
            set {
                _linker.Language = value;
                SavePrefs();
            }
        }

        public void SavePrefs() {
            EditorPrefs.SetString(CONST.SETTING_DATA_KEY_NAME, JsonUtility.ToJson(_linker));
        }

        SettingData LoadAssetInfo() {
            // データがない
            if (!EditorPrefs.HasKey(CONST.SETTING_DATA_KEY_NAME))
                return new SettingData();

            string jsonData = EditorPrefs.GetString(CONST.SETTING_DATA_KEY_NAME);

            // json から読み込む
            var assets = JsonUtility.FromJson<SettingData>(jsonData);
            if (assets == null) {
                return new SettingData();
            }
            return assets;
        }
    }
}
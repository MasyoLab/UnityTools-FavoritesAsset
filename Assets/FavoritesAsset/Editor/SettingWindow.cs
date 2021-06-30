using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasyoLab.Editor.FavoritesAsset {

    class SettingWindow {
        FavoritesManager _manager = null;

        public SettingWindow() { }
        public SettingWindow(FavoritesManager manager) {
            SetData(manager);
        }

        public void SetData(FavoritesManager manager) {
            _manager = manager;
        }

        /// <summary>
        /// プルダウンメニュー
        /// </summary>
        public void SettingGUI() {
            EditorGUI.BeginChangeCheck();
            _manager.Language = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_manager.Language, TextEnum.Language), (int)_manager.Language, LanguageData.LANGUAGE);
            EditorGUI.EndChangeCheck();
        }
    }
}
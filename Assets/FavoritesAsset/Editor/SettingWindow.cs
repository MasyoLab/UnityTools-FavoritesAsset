using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasyoLab.Editor.FavoritesAsset {

    class SettingWindow : BaseWindow {

        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();
            _manager.Language = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_manager.Language, TextEnum.Language), (int)_manager.Language, LanguageData.LANGUAGE);
            _manager.SavePrefs();
            EditorGUI.EndChangeCheck();
        }
    }
}
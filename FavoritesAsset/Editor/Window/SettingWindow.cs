using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasyoLab.Editor.FavoritesAsset {

    class SettingWindow : BaseWindow {

        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();
            _manager.Language = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_manager.Language, TextEnum.Language), (int)_manager.Language, LanguageData.LANGUAGE);
            EditorGUI.EndChangeCheck();

            // お気に入り全解除
            var content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.UnlockAll));
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                _manager.RemoveAll();
            }
        }
    }
}
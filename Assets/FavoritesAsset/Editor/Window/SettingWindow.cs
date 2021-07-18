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

    class SettingWindow : BaseWindow {

        Vector2 _scrollVec2;

        public override void OnGUI() {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);

            EditorGUI.BeginChangeCheck();
            _manager.Language = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_manager.Language, TextEnum.Language), (int)_manager.Language, LanguageData.LANGUAGE);
            EditorGUI.EndChangeCheck();
            Utils.GUILine();

            // お気に入り全解除
            var content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.UnlockAll));
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                _manager.RemoveAll();
                _manager.SaveFavoritesData();
            }
            Utils.GUILine();

            GUILayout.Label(LanguageData.GetText(_manager.Language, TextEnum.ImportAndExportTarget));
            EditorGUILayout.TextField(LanguageData.GetText(_manager.Language, TextEnum.ExportTarget), _manager.ExportTarget);
            EditorGUILayout.TextField(LanguageData.GetText(_manager.Language, TextEnum.ImportTarget), _manager.ImportTarget);
            Utils.GUILine();

            GUILayout.EndScrollView();
        }
    }
}
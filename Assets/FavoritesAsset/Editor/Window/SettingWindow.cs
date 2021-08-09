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

        public override void OnGUI(Rect windowSize) {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);

            EditorGUI.BeginChangeCheck();
            _setting.Language = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_setting.Language, TextEnum.Language), (int)_setting.Language, LanguageData.LANGUAGE);
            EditorGUI.EndChangeCheck();
            Utils.GUILine();

            // お気に入り全解除
            var content = new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.UnlockAll));
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                _favorites.RemoveAll();
                _favorites.SaveFavoritesData();
            }

            Utils.GUILine();

            GUILayout.Label(LanguageData.GetText(_setting.Language, TextEnum.ImportAndExportTarget));

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(_setting.Language, TextEnum.ExportTarget));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(_setting.ExportTarget);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(_setting.Language, TextEnum.ImportTarget));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(_setting.ImportTarget);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            Utils.GUILine();
            GUILayout.EndScrollView();
        }
    }
}
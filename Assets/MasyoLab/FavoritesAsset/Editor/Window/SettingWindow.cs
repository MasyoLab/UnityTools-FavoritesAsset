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
            {
                var newLanguage = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Language), (int)_pipeline.Setting.Language, LanguageData.LANGUAGE);
                var isUpdate = _pipeline.Setting.Language != newLanguage;
                _pipeline.Setting.Language = newLanguage;
                if (isUpdate) {
                    _pipeline.Group.UpdateGroupNameList();
                }
            }
            EditorGUI.EndChangeCheck();

            Utils.GUILine();

            {
                // お気に入り全解除
                GUILayout.Label($"{LanguageData.GetText(_pipeline.Setting.Language, TextEnum.FavoriteGroup)} : " +
                    $"{_pipeline.Group.GetGroupNameByGUID(_pipeline.Group.SelectGroupFileName)}");
                var content = new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.UnlockAll));
                if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                    _pipeline.Favorites.RemoveAll();
                    _pipeline.Favorites.SaveFavoritesData();
                    (_pipeline.Root as MainWindow).Reload();
                }
            }

            Utils.GUILine();
            GUILayout.Label(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.ImportAndExportTarget));

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.ExportTarget));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(_pipeline.Setting.IOTarget);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.ImportTarget));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(_pipeline.Setting.IOTarget);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Filename));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(_pipeline.Setting.IOFileName);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            Utils.GUILine();
            GUILayout.EndScrollView();
        }
    }
}

#if UNITY_EDITOR
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

        private Vector2 m_scrollVec2 = Vector2.zero;

        public override void OnGUI() {
            m_scrollVec2 = GUILayout.BeginScrollView(m_scrollVec2);

            EditorGUI.BeginChangeCheck();
            {
                var newLanguage = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.Language), (int)m_pipeline.Setting.Language, LanguageData.LANGUAGE);
                var isUpdate = m_pipeline.Setting.Language != newLanguage;
                m_pipeline.Setting.Language = newLanguage;
                if (isUpdate) {
                    m_pipeline.Group.UpdateGroupNameList();
                }
            }
            EditorGUI.EndChangeCheck();

            Utils.GUILine();

            {
                // お気に入り全解除
                GUILayout.Label($"{LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.FavoriteGroup)} : " +
                    $"{m_pipeline.Group.GetGroupNameByGUID(m_pipeline.Group.SelectGroupFileName)}");
                var content = new GUIContent(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.UnlockAll));
                if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                    m_pipeline.Favorites.RemoveAll();
                    m_pipeline.Favorites.SaveFavoritesData();
                    (m_pipeline.Root as MainWindow).Reload();
                }
            }

            Utils.GUILine();
            GUILayout.Label(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.ImportAndExportTarget));

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.ExportTarget));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(m_pipeline.Setting.IOTarget);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.ImportTarget));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(m_pipeline.Setting.IOTarget);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.Filename));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(m_pipeline.Setting.IOFileName);
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            Utils.GUILine();
            GUILayout.EndScrollView();
        }
    }
}
#endif

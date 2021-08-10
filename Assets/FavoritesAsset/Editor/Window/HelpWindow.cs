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

    class HelpWindow : BaseWindow {

        Vector2 _scrollVec2;

        static GUIStyle _headerStyle = new GUIStyle(EditorStyles.label) {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20
        };

        static GUIStyle _h1 = new GUIStyle(EditorStyles.label) {
            fontStyle = FontStyle.Bold,
            fontSize = 18
        };

        static GUIStyle _linkLabel = new GUIStyle(EditorStyles.linkLabel) {
            fontStyle = FontStyle.Bold,
            fontSize = 15
        };

        public override void OnGUI(Rect windowSize) {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);

            GUILayout.Label(CONST.EDITOR_WINDOW_NAME, _headerStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label(CONST.VERSION, _headerStyle, GUILayout.ExpandWidth(true));
            Utils.GUILine();

            GUILayout.Label(LanguageData.GetText(_setting.Language, TextEnum.Link), _h1, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Readme", _linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/README.md");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button(LanguageData.GetText(_setting.Language, TextEnum.License), _linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/LICENSE");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button(LanguageData.GetText(_setting.Language, TextEnum.LatestRelease), _linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/releases");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button(LanguageData.GetText(_setting.Language, TextEnum.SourceCode), _linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset");
            }
            Utils.MouseCursorLink();

            GUILayout.EndScrollView();
        }
    }
}
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

        GUIStyle _headerStyle = new GUIStyle(EditorStyles.label) {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };

        public override void OnGUI() {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);

            GUILayout.Label(CONST.EDITOR_WINDOW_NAME, _headerStyle, GUILayout.ExpandWidth(true));
            Utils.GUILine();

            if (GUILayout.Button("README", EditorStyles.linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/README.md");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button("License", EditorStyles.linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/LICENSE");
            }
            Utils.MouseCursorLink();

            GUILayout.EndScrollView();
        }
    }
}
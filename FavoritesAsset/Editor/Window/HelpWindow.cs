using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasyoLab.Editor.FavoritesAsset {
    class HelpWindow : BaseWindow {

        GUIStyle _headerStyle = new GUIStyle(EditorStyles.label) {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };

        public override void OnGUI() {
            GUILayout.Label(CONST.EDITOR_WINDOW_NAME, _headerStyle, GUILayout.ExpandWidth(true));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

            if (GUILayout.Button("README", EditorStyles.linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/README.md");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button("License", EditorStyles.linkLabel, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/LICENSE");
            }
            Utils.MouseCursorLink();
        }
    }
}
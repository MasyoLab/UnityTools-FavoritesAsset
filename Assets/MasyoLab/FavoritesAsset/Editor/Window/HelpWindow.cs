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

        PtrLinker<GUIStyle> _headerStyle = new PtrLinker<GUIStyle>(() => {
            return new GUIStyle(EditorStyles.label) {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20
            };
        });

        PtrLinker<GUIStyle> _h1 = new PtrLinker<GUIStyle>(() => {
            return new GUIStyle(EditorStyles.label) {
                fontStyle = FontStyle.Bold,
                fontSize = 18
            };
        });

        PtrLinker<GUIStyle> _linkLabel = new PtrLinker<GUIStyle>(() => {
            return new GUIStyle(EditorStyles.linkLabel) {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };
        });

        public override void OnGUI(Rect windowSize) {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);

            GUILayout.Label(CONST.EDITOR_WINDOW_NAME, _headerStyle.Inst, GUILayout.ExpandWidth(true));
            GUILayout.Label(CONST.VERSION, _headerStyle.Inst, GUILayout.ExpandWidth(true));
            Utils.GUILine();

            GUILayout.Label(LanguageData.GetText(_setting.Language, TextEnum.Link), _h1.Inst, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Readme", _linkLabel.Inst, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/README.md");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button(LanguageData.GetText(_setting.Language, TextEnum.License), _linkLabel.Inst, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/blob/master/LICENSE.md");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button(LanguageData.GetText(_setting.Language, TextEnum.LatestRelease), _linkLabel.Inst, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset/releases");
            }
            Utils.MouseCursorLink();

            if (GUILayout.Button(LanguageData.GetText(_setting.Language, TextEnum.SourceCode), _linkLabel.Inst, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false))) {
                Utils.OpenURL("https://github.com/MasyoLab/UnityTools-FavoritesAsset");
            }
            Utils.MouseCursorLink();

            GUILayout.EndScrollView();
        }
    }
}

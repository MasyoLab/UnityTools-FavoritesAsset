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

    struct AssetDrawer {

        static GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);

        /// <summary>
        /// アセットの情報を描画
        /// </summary>
        /// <param name="info"></param>
        /// <param name="onAction"></param>
        static void DrawingSetting(AssetInfo info, UnityEngine.Events.UnityAction<GUIContent, GUIStyle> onAction = null) {
            // 名前を使う
            var content = new GUIContent(info.Name, AssetDatabase.GetCachedIcon(info.Path));

            var style = ButtonStyle;
            var originalAlignment = style.alignment;
            var originalFontStyle = style.fontStyle;
            var originalTextColor = style.normal.textColor;

            style.alignment = TextAnchor.MiddleLeft;

            onAction?.Invoke(content, style);

            style.alignment = originalAlignment;
            style.fontStyle = originalFontStyle;
            style.normal.textColor = originalTextColor;
        }

        /// <summary>
        /// アセットを開くボタン
        /// </summary>
        /// <param name="info"></param>
        public static void OnAssetButton(Rect rect, AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            DrawingSetting(info, (content, style) => {
                if (GUI.Button(rect, content, style)) {
                    onButtonAction?.Invoke(info);
                }
            });
        }

        /// <summary>
        /// アセットを開くボタン
        /// </summary>
        /// <param name="info"></param>
        public static void OnAssetButton(EditorWindow win, AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            DrawingSetting(info, (content, style) => {
                float width = win.position.width - 100f;
                if (GUILayout.Button(content, style, GUILayout.MaxWidth(width), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                    onButtonAction?.Invoke(info);
                }
            });
        }

        /// <summary>
        /// アセットを開くボタン
        /// </summary>
        /// <param name="info"></param>
        public static void OnAssetButton(AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            DrawingSetting(info, (content, style) => {
                if (GUILayout.Button(content, style, GUILayout.ExpandWidth(true), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                    onButtonAction?.Invoke(info);
                }
            });
        }

        /// <summary>
        /// アセットをPingする
        /// </summary>
        /// <param name="info"></param>
        public static void OnPingObjectButton(AssetInfo info) {
            // アイコンを指定
            var content = EditorGUIUtility.IconContent(CONST.ICON_ANIMATION_VISIBILITY_TOGGLE_ON);
            // ボタン
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                // アセットの情報
                var asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
                EditorGUIUtility.PingObject(asset);
            }
        }

        /// <summary>
        /// お気に入り解除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool OnUnfavoriteButton(AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            // アイコンを指定
            var content = EditorGUIUtility.IconContent(CONST.ICON_CLOSE);
            // ボタン
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                onButtonAction?.Invoke(info);
                return true;
            }
            return false;
        }
    }
}
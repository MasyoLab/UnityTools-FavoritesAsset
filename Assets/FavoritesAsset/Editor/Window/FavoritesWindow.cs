using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    /// <summary>
    /// お気に入り登録機能
    /// </summary>
    class FavoritesWindow : BaseWindow {

        static Vector2 _scrollVec2;
        const float TEXT_PACE = 20;

        public override void OnGUI(Rect windowSize) {
            DrawBG(windowSize.x, windowSize.y + (TEXT_PACE * 2), windowSize.width, windowSize.height - (TEXT_PACE * 2));

            // ドラッグアンドドロップ
            DragAndDropGUI();

            // アセット表示
            DrawAssetGUI();

            GUI.Box(GUILayoutUtility.GetRect(0, TEXT_PACE, GUILayout.ExpandWidth(true), GUILayout.Height(TEXT_PACE)), $"{_manager.Data.Count} {LanguageData.GetText(_manager.Language, TextEnum.NumFav)}");
        }

        void DrawBG(float posX, float posY, float width, float height) {
            { // 左上
                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 右下レイアウト
                style.alignment = TextAnchor.LowerRight;
                GUI.Label(new Rect(width * 0f, posY, width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED_D)
                    );
                style.alignment = originalAlignment;
            }
            { // 右上

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.LowerLeft;
                GUI.Label(new Rect(width * 0.5f, posY, width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED)
                    );
                style.alignment = originalAlignment;
            }
            { // 左下

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.UpperRight;
                GUI.Label(new Rect(width * 0f, posY + (height * 0.5f), width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED)
                    );
                style.alignment = originalAlignment;
            }
            { // 右下

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.UpperLeft;
                GUI.Label(new Rect(width * 0.5f, posY + (height * 0.5f), width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED_D)
                    );
                style.alignment = originalAlignment;
            }
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        void DragAndDropGUI() {
            GUI.Box(GUILayoutUtility.GetRect(0, TEXT_PACE, GUILayout.ExpandWidth(true)), LanguageData.GetText(_manager.Language, TextEnum.DragAndDrop));

            if (!GetObjects(out List<string> objs, _root))
                return;

            foreach (var item in objs) {
                AddAssetToAssetPath(item);
            }

            _manager.SaveFavoritesData();
        }

        /// <summary>
        /// アセット表示
        /// </summary>
        void DrawAssetGUI() {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            foreach (var info in _manager.Data) {
                // お気に入り登録したアセットを表示
                if (DrawAssetRow(info))
                    break;
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// お気に入りの描画
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool DrawAssetRow(AssetInfo info) {
            GUILayout.BeginHorizontal();

            // Ping を実行
            AssetDrawer.OnPingObjectButton(info);

            // アセットを開くボタン
            AssetDrawer.OnAssetButton(_root, info, OpenAsset);

            // お気に入り除ボタン
            bool result = AssetDrawer.OnUnfavoriteButton(info, RemoveAsset);

            GUILayout.EndHorizontal();

            return result;
        }

        /// <summary>
        /// ファイルパスでアセットを登録
        /// </summary>
        void AddAssetToAssetPath(string assetPath) {
            // AssetPathは保存済み
            if (_manager.ExistsAssetPath(assetPath))
                return;

            // GUID を取得
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            // お気に入りに登録
            _manager.Add(guid, assetPath, asset.name, asset.GetType().ToString());
        }

        /// <summary>
        /// GUIDでアセットを登録
        /// </summary>
        void AddAssetToGUID(string assetGuid) {
            // GUIDは保存済み
            if (_manager.ExistsGUID(assetGuid))
                return;

            // GUID からアセットパスを取得
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            // お気に入りに登録
            _manager.Add(assetGuid, path, asset.name, asset.GetType().ToString());
        }

        /// <summary>
        /// お気に入りを解除
        /// </summary>
        /// <param name="info"></param>
        void RemoveAsset(AssetInfo info) {
            _manager.Remove(info);
            _manager.SaveFavoritesData();
        }

        /// <summary>
        /// アセットを開く
        /// </summary>
        /// <param name="info"></param>
        void OpenAsset(AssetInfo info) {
            // シーンアセットを開こうとしている
            if (Path.GetExtension(info.Path).Equals(CONST.UNITY_EXT)) {
                // シーンの保存を促す
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    // 保存をしたので開く
                    EditorSceneManager.OpenScene(info.Path, OpenSceneMode.Single);
                }
                return;
            }

            var asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
            // アセットを開く
            AssetDatabase.OpenAsset(asset);
        }

        /// <summary>
        /// ドラッグ&ドロップでオブジェクトを取得
        /// </summary>
        public static bool GetObjects(out List<string> targetList, EditorWindow window, Rect? rect = null) {
            targetList = null;

            // ドラックドロップされたオブジェクトがなければ終わり
            var objectReferences = GetObjects(window, rect);
            if (objectReferences == null)
                return false;

            // ドロップされたオブジェクトに対象の物が無ければ終わり
            targetList = objectReferences.ToList();
            if (targetList.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// ドラッグ&ドロップでオブジェクトを取得
        /// </summary>
        /// <param name="areaTitle"></param>
        /// <param name="widthMin"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static string[] GetObjects(EditorWindow window, Rect? rect = null) {
            var ev = Event.current;

            // エリアが指定されていれば範囲内か確認
            if (rect.HasValue && !rect.Value.Contains(ev.mousePosition))
                return null;

            // ドラッグ＆ドロップで操作が更新された時でも、実行した時でもなければ終わり
            if (ev.type != EventType.DragUpdated && ev.type != EventType.DragPerform)
                return null;

            var paths = DragAndDrop.paths;
            if (EditorWindow.mouseOverWindow != window || paths.Length <= 0)
                return null;

            // カーソルに「+」のアイコンを表示
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            // ドラッグ&ドロップで無ければ終わり
            if (ev.type != EventType.DragPerform)
                return null;

            //ドラッグを受け付ける
            DragAndDrop.AcceptDrag();

            DragAndDrop.activeControlID = 0;

            //イベントを使用済みにする
            Event.current.Use();

            return DragAndDrop.paths;
        }
    }
}
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
    public class FavoritesAsset : EditorWindow {

        /// <summary>
        /// マネージャー
        /// </summary>
        static FavoritesManager _ref = null;
        FavoritesManager _manager {
            get {
                if (_ref == null) {
                    _ref = new FavoritesManager();
                }
                return _ref;
            }
        }

        static Vector2 _scrollVec2;

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        static void Init() {
            var window = GetWindow<FavoritesAsset>(CONST.EDITOR_NAME);
            window.titleContent.image = EditorGUIUtility.IconContent(CONST.FAVORITE_ICON).image;
        }

        /// <summary>
        /// GUI 描画
        /// </summary>
        void OnGUI() {
            DrawBG();

            DrawToolbar();

            // ドラッグアンドドロップ
            DragAndDropGUI();

            // プルダウンメニュー
            PulldownMenuGUI();

            // メニュー
            MenuGUI();

            GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));

            // アセット表示
            DrawAssetGUI();

            GUI.Box(GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.Height(20)), $"{_manager.Data.Count} {LanguageData.GetText(_manager.Language, TextEnum.NumFav)}");
        }

        void OnFocus() {
            _manager.CheckFavoritesAsset();
        }

        void DrawBG() {
            { // 左上
                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 右下レイアウト
                style.alignment = TextAnchor.LowerRight;
                GUI.Label(new Rect(position.width * 0f, position.height * 0f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED_D)
                    );
                style.alignment = originalAlignment;
            }
            { // 右上

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.LowerLeft;
                GUI.Label(new Rect(position.width * 0.5f, position.height * 0f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED)
                    );
                style.alignment = originalAlignment;
            }
            { // 左下

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.UpperRight;
                GUI.Label(new Rect(position.width * 0f, position.height * 0.5f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED)
                    );
                style.alignment = originalAlignment;
            }
            { // 右下

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.UpperLeft;
                GUI.Label(new Rect(position.width * 0.5f, position.height * 0.5f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED_D)
                    );
                style.alignment = originalAlignment;
            }
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        void DragAndDropGUI() {
            GUI.Box(GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true)), LanguageData.GetText(_manager.Language, TextEnum.DragAndDrop));

            if (!GetObjects(out List<string> objs, this))
                return;

            foreach (var item in objs) {
                AddAssetToAssetPath(item);
            }

            _manager.SavePrefs();
        }

        /// <summary>
        /// プルダウンメニュー
        /// </summary>
        void PulldownMenuGUI() {
            EditorGUI.BeginChangeCheck();
            _manager.Language = (LanguageEnum)EditorGUILayout.Popup(LanguageData.GetText(_manager.Language, TextEnum.Language), (int)_manager.Language, LanguageData.LANGUAGE);
            EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// メニュー
        /// </summary>
        void MenuGUI() {
            GUILayout.BeginHorizontal();
            {
                var content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.UnlockAll));
                // お気に入り全解除
                if (GUILayout.Button(content, GUILayout.ExpandWidth(true), GUILayout.Height(40))) {
                    _manager.RemoveAll();
                }
            }
            {
                var content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.ChangeDisplay));
                if (GUILayout.Button(content, GUILayout.ExpandWidth(true), GUILayout.Height(40))) {
                    SortWindow.Display(this, _manager);
                }
            }
            GUILayout.EndHorizontal();
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
            AssetDrawer.OnAssetButton(this, info, OpenAsset);

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
            _manager.SavePrefs();
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
            if (mouseOverWindow != window || paths.Length <= 0)
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

        //上のツールバーを表示する
        private void DrawToolbar() {

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.MinWidth(1))) {
                if (GUILayout.Button("File", EditorStyles.toolbarDropDown)) {
                    OpenMenu(Vector2.zero);
                }
            }
        }

        void OpenMenu(Vector2 mousePos) {

            Rect contextRect = new Rect(0, 0, Screen.width, Screen.height);
            if (contextRect.Contains(mousePos)) {
                // Now create the menu, add items and show it
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Import)), false, (call) => { _manager.SetJsonData(SaveLoad.Load()); }, "item 1");
                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Export)), false, (call) => { SaveLoad.Save(_manager.AssetDBJson); }, "item 2");
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, call => { }, "item 3");

                menu.ShowAsContext();
            }
        }
    }
}
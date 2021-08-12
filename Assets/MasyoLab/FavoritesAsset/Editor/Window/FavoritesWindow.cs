using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

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

        static GUIStyle _backgroundStyle = new GUIStyle(EditorStyles.label) {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        static GUIStyle _boxStyle = new GUIStyle(GUI.skin.textArea) {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        static Vector2 _scrollVec2;
        ReorderableList _reorderableList = null;

        public override void Init(PtrLinker<SystemManager> manager, EditorWindow root) {
            base.Init(manager, root);
            InitSortFunction();
        }

        public override void OnGUI(Rect windowSize) {
            //DrawBG(windowSize.x, windowSize.y + (CONST.GUI_LAYOUT_HEIGHT * 2), windowSize.width, windowSize.height - (CONST.GUI_LAYOUT_HEIGHT * 2));

            // ドラッグアンドドロップ
            DragAndDropGUI();

            // アセット表示
            DrawAssetGUI();

            GUI.Box(GUILayoutUtility.GetRect(0, CONST.GUI_LAYOUT_HEIGHT, GUILayout.ExpandWidth(true)),
                $"{_favorites.Data.Count} {LanguageData.GetText(_setting.Language, TextEnum.NumFav)}",
                _boxStyle);
        }

        public override void Reload() {
            _reorderableList = null;
            InitSortFunction();
        }

        void InitSortFunction() {
            if (_reorderableList != null)
                return;

            // データを複製
            var assetInfos = _favorites.Data.ToList();
            AssetInfo releaseTarget = null;

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list) {
                _favorites.SortData(assetInfos);
            }

            void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, "");

                // ヘッダーは最初に処理されるのでここでデータ数の確認
                if (assetInfos.Count != _favorites.Data.Count) {
                    assetInfos = _favorites.Data.ToList();
                    _reorderableList.list = assetInfos;
                }
            }

            void DrawFooter(Rect rect) {
                EditorGUI.LabelField(rect, "");
                // フッターで解放する
                if (releaseTarget != null) {
                    RemoveAsset(releaseTarget);
                    assetInfos = _favorites.Data.ToList();
                    _reorderableList.list = assetInfos;
                    releaseTarget = null;
                }
            }

            void NoneElement(Rect rect) {
                EditorGUI.LabelField(rect, "");
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                // お気に入り解除時に実行
                if (DrawAsset(rect, index, isActive, isFocused)) {
                    releaseTarget = _favorites.Data[index];
                }
            }

            _reorderableList = new ReorderableList(assetInfos, typeof(AssetInfo)) {
                drawElementCallback = DrawElement,
                onChangedCallback = Changed,
                drawHeaderCallback = DrawHeader,
                drawFooterCallback = DrawFooter,
                drawNoneElementCallback = NoneElement,
            };
            _reorderableList.headerHeight = 0;
            _reorderableList.footerHeight = 0;
            _reorderableList.elementHeight = CONST.GUI_LAYOUT_HEIGHT;
            _reorderableList.showDefaultBackground = false;
        }

        void DrawBG(float posX, float posY, float width, float height) {
            {
                // 右下レイアウト
                _backgroundStyle.alignment = TextAnchor.LowerRight;
                GUI.Label(new Rect(width * 0f, posY, width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED_D), _backgroundStyle);
            }
            {
                // 左下レイアウト
                _backgroundStyle.alignment = TextAnchor.LowerLeft;
                GUI.Label(new Rect(width * 0.5f, posY, width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED), _backgroundStyle);
            }
            {
                // 左下レイアウト
                _backgroundStyle.alignment = TextAnchor.UpperRight;
                GUI.Label(new Rect(width * 0f, posY + (height * 0.5f), width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED), _backgroundStyle);
            }
            {
                // 左下レイアウト
                _backgroundStyle.alignment = TextAnchor.UpperLeft;
                GUI.Label(new Rect(width * 0.5f, posY + (height * 0.5f), width * 0.5f, height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED_D), _backgroundStyle);
            }

            _backgroundStyle.alignment = TextAnchor.MiddleCenter;
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        void DragAndDropGUI() {
            GUI.Box(GUILayoutUtility.GetRect(0, CONST.GUI_LAYOUT_HEIGHT, GUILayout.ExpandWidth(true)),
                LanguageData.GetText(_setting.Language, TextEnum.DragAndDrop), _boxStyle);

            if (!GetObjects(out List<string> objs, _root))
                return;

            foreach (var item in objs) {
                AddAssetToAssetPath(item);
            }

            _favorites.SaveFavoritesData();
        }

        /// <summary>
        /// アセット表示
        /// </summary>
        void DrawAssetGUI() {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// お気に入りの描画
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool DrawAsset(Rect rect, int index, bool isActive, bool isFocused) {
            float BUTTON_WIDTH = 30;
            var assetInfo = _favorites.Data[index];
            var copyRect = rect;

            copyRect.width = BUTTON_WIDTH;

            // Ping を実行
            AssetDrawer.OnPingObjectButton(copyRect, assetInfo);

            rect.x += BUTTON_WIDTH;
            rect.width -= BUTTON_WIDTH * 2;

            // アセットを開くボタン
            AssetDrawer.OnAssetButton(rect, assetInfo, OpenAsset);

            copyRect.x = rect.x + rect.width;

            // お気に入り除ボタン
            return AssetDrawer.OnUnfavoriteButton(copyRect, assetInfo);
        }

        /// <summary>
        /// ファイルパスでアセットを登録
        /// </summary>
        void AddAssetToAssetPath(string assetPath) {
            // AssetPathは保存済み
            if (_favorites.ExistsAssetPath(assetPath))
                return;

            // GUID を取得
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            // お気に入りに登録
            _favorites.Add(guid, assetPath, asset.name, asset.GetType().ToString());
        }

        /// <summary>
        /// Objectでアセットを登録
        /// </summary>
        public void AddAssetToObject(Object assetObject) {
            var assetPath = AssetDatabase.GetAssetPath(assetObject);

            // AssetPathは保存済み
            if (_favorites.ExistsAssetPath(assetPath))
                return;

            // GUID を取得
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            // お気に入りに登録
            _favorites.Add(guid, assetPath, assetObject.name, assetObject.GetType().ToString());
        }

        /// <summary>
        /// GUIDでアセットを登録
        /// </summary>
        void AddAssetToGUID(string assetGuid) {
            // GUIDは保存済み
            if (_favorites.ExistsGUID(assetGuid))
                return;

            // GUID からアセットパスを取得
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            // お気に入りに登録
            _favorites.Add(assetGuid, path, asset.name, asset.GetType().ToString());
        }

        /// <summary>
        /// お気に入りを解除
        /// </summary>
        /// <param name="info"></param>
        void RemoveAsset(AssetInfo info) {
            _favorites.Remove(info);
            _favorites.SaveFavoritesData();
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

        public void Save() {
            _favorites.SaveFavoritesData();
        }
    }
}

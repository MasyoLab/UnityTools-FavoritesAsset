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

        PtrLinker<GUIStyle> _boxStyle = new PtrLinker<GUIStyle>(() => {
            return new GUIStyle(GUI.skin.textArea) {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
        });

        static Vector2 _scrollVec2;
        ReorderableList _reorderableList = null;

        public override void Init(IPipeline pipeline) {
            base.Init(pipeline);
            InitSortFunction();
        }

        public override void OnGUI() {
            // ドラッグアンドドロップ
            DragAndDropGUI();

            // アセット表示
            DrawAssetGUI();

            GUI.Box(GUILayoutUtility.GetRect(0, CONST.GUI_LAYOUT_HEIGHT, GUILayout.ExpandWidth(true)),
                $"{_pipeline.Favorites.Data.Count} {LanguageData.GetText(_pipeline.Setting.Language, TextEnum.NumFav)}",
                _boxStyle.Inst);
        }

        public override void Reload() {
            InitSortFunction();
        }

        void InitSortFunction() {
            // データを複製
            var assetDatas = _pipeline.Favorites.Data.ToList();

            if (_reorderableList == null) {
                _reorderableList = new ReorderableList(assetDatas, typeof(AssetData));
            }

            AssetData releaseTarget = null;

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list) {
                _pipeline.Favorites.SortData(assetDatas);
            }

            void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, "");

                // ヘッダーは最初に処理されるのでここでデータ数の確認
                if (assetDatas.Count != _pipeline.Favorites.Data.Count) {
                    assetDatas = _pipeline.Favorites.Data.ToList();
                    _reorderableList.list = assetDatas;
                }
            }

            void DrawFooter(Rect rect) {
                EditorGUI.LabelField(rect, "");
                // フッターで解放する
                if (releaseTarget != null) {
                    RemoveAsset(releaseTarget);
                    assetDatas = _pipeline.Favorites.Data.ToList();
                    _reorderableList.list = assetDatas;
                    releaseTarget = null;
                }
            }

            void NoneElement(Rect rect) {
                EditorGUI.LabelField(rect, "");
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                // お気に入り解除時に実行
                if (DrawAsset(rect, index, isActive, isFocused)) {
                    releaseTarget = _pipeline.Favorites.Data[index];
                }
            }

            _reorderableList.list = assetDatas;

            _reorderableList.drawElementCallback = DrawElement;
            _reorderableList.onChangedCallback = Changed;
            _reorderableList.drawHeaderCallback = DrawHeader;
            _reorderableList.drawFooterCallback = DrawFooter;
            _reorderableList.drawNoneElementCallback = NoneElement;

            _reorderableList.headerHeight = 0;
            _reorderableList.footerHeight = 0;
            _reorderableList.elementHeight = CONST.GUI_LAYOUT_HEIGHT;
            _reorderableList.showDefaultBackground = false;
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        void DragAndDropGUI() {
            GUI.Box(GUILayoutUtility.GetRect(0, CONST.GUI_LAYOUT_HEIGHT, GUILayout.ExpandWidth(true)),
                LanguageData.GetText(_pipeline.Setting.Language, TextEnum.DragAndDrop), _boxStyle.Inst);

            if (!GetObjects(out List<UnityEngine.Object> objs, _pipeline.Root))
                return;

            foreach (var item in objs) {
                AddAssetToObject(item);
            }

            _pipeline.Favorites.SaveFavoritesData();
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
            var assetData = _pipeline.Favorites.Data[index];
            var copyRect = rect;

            copyRect.width = BUTTON_WIDTH;

            // Ping を実行
            AssetDrawer.OnPingObjectButton(copyRect, assetData);

            rect.x += BUTTON_WIDTH;
            rect.width -= BUTTON_WIDTH * 2;

            // アセットを開くボタン
            AssetDrawer.OnAssetButton(rect, assetData, OpenAsset);

            copyRect.x = rect.x + rect.width;

            // お気に入り除ボタン
            return AssetDrawer.OnUnfavoriteButton(copyRect, assetData);
        }

        /// <summary>
        /// ファイルパスでアセットを登録
        /// </summary>
        /*
        void AddAssetToAssetPath(string assetPath) {
            // AssetPathは保存済み
            if (_pipeline.Favorites.ExistsAssetPath(assetPath))
                return;

            // GUID を取得
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string oguid, out long localid)) { }

            // お気に入りに登録
            _pipeline.Favorites.Add(guid, assetPath, asset.name, asset.GetType().ToString(), localid);
        }
        */

        /// <summary>
        /// Objectでアセットを登録
        /// </summary>
        public void AddAssetToObject(Object assetObject) {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assetObject, out string guid, out long localid))
                return;

            // Assetは保存済み
            if (_pipeline.Favorites.ExistsGUID(guid, localid))
                return;

            var assetPath = AssetDatabase.GetAssetPath(assetObject);

            // SubAssets が1つしか無い場合
            var assetDatas = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            if (assetDatas.Length == 1) {
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assetDatas[0], out string sbuGuid, out long sbuLocalid))
                    return;
                // Assetは保存済み
                if (_pipeline.Favorites.ExistsGUID(sbuGuid, sbuLocalid))
                    return;
                guid = sbuGuid;
                localid = sbuLocalid;
                assetObject = assetDatas[0];
            }

            // お気に入りに登録
            _pipeline.Favorites.Add(guid, assetPath, assetObject.name, assetObject.GetType().ToString(), localid);
        }

        /// <summary>
        /// GUIDでアセットを登録
        /// </summary>
        /*
        void AddAssetToGUID(string assetGuid) {
            // GUIDは保存済み
            if (_pipeline.Favorites.ExistsGUID(assetGuid))
                return;

            // GUID からアセットパスを取得
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string oguid, out long localid)) { }

            // お気に入りに登録
            _pipeline.Favorites.Add(assetGuid, path, asset.name, asset.GetType().ToString(), localid);
        }
        */

        /// <summary>
        /// お気に入りを解除
        /// </summary>
        /// <param name="info"></param>
        void RemoveAsset(AssetData info) {
            _pipeline.Favorites.Remove(info);
            _pipeline.Favorites.SaveFavoritesData();
        }

        /// <summary>
        /// アセットを開く
        /// </summary>
        /// <param name="info"></param>
        void OpenAsset(AssetData info) {
            // シーンアセットを開こうとしている
            if (Path.GetExtension(info.Path).Equals(CONST.UNITY_EXT)) {
                // シーンの保存を促す
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    // 保存をしたので開く
                    EditorSceneManager.OpenScene(info.Path, OpenSceneMode.Single);
                }
                return;
            }

            // アセットを開く
            AssetDatabase.OpenAsset(info.GetObject());
        }

        /// <summary>
        /// ドラッグ&ドロップでオブジェクトを取得
        /// </summary>
        public static bool GetObjects(out List<UnityEngine.Object> targetList, EditorWindow window, Rect? rect = null) {
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
        private static UnityEngine.Object[] GetObjects(EditorWindow window, Rect? rect = null) {
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

            return DragAndDrop.objectReferences;
        }

        public void Save() {
            _pipeline.Favorites.SaveFavoritesData();
        }
    }
}

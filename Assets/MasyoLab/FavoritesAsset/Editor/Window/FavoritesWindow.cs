#if UNITY_EDITOR
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

namespace MasyoLab.Editor.FavoritesAsset
{
    /// <summary>
    /// お気に入り登録機能
    /// </summary>
    class FavoritesWindow : BaseWindow
    {
        private PtrLinker<GUIStyle> m_boxStyle = new PtrLinker<GUIStyle>(() =>
        {
            return new GUIStyle(GUI.skin.textArea)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
        });

        private static Vector2 m_scrollVec = Vector2.zero;
        private ReorderableList m_reorderableList = null;

        public override void Init(IPipeline pipeline)
        {
            base.Init(pipeline);
            InitFavoritesWindow();
        }

        public override void OnGUI()
        {
            // ドラッグアンドドロップ
            DragAndDropGUI();

            // アセット表示
            DrawAssetGUI();

            GUI.Box(GUILayoutUtility.GetRect(0, CONST.GUI_LAYOUT_HEIGHT, GUILayout.ExpandWidth(true)),
                $"{m_pipeline.Favorites.Data.Count} {LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.NumFav)}",
                m_boxStyle.Inst);
        }

        public override void Reload()
        {
            InitFavoritesWindow();
        }

        private void InitFavoritesWindow()
        {
            // データを複製
            var assetDatas = m_pipeline.Favorites.Data.ToList();
            assetDatas.ForEach(v => v.UpdateData());

            if (m_reorderableList == null)
            {
                m_reorderableList = new ReorderableList(assetDatas, typeof(AssetData));
            }

            AssetData releaseTarget = null;

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list)
            {
                m_pipeline.Favorites.SortData(assetDatas);
            }

            void DrawHeader(Rect rect)
            {
                EditorGUI.LabelField(rect, string.Empty);

                // ヘッダーは最初に処理されるのでここでデータ数の確認
                if (assetDatas.Count != m_pipeline.Favorites.Data.Count)
                {
                    assetDatas = m_pipeline.Favorites.Data.ToList();
                    m_reorderableList.list = assetDatas;
                }
            }

            void DrawFooter(Rect rect)
            {
                EditorGUI.LabelField(rect, string.Empty);
                // フッターで解放する
                if (releaseTarget != null)
                {
                    RemoveAsset(releaseTarget);
                    assetDatas = m_pipeline.Favorites.Data.ToList();
                    m_reorderableList.list = assetDatas;
                    releaseTarget = null;
                }
            }

            void NoneElement(Rect rect)
            {
                EditorGUI.LabelField(rect, string.Empty);
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                // お気に入り解除時に実行
                if (DrawAsset(rect, index, isActive, isFocused))
                {
                    releaseTarget = m_pipeline.Favorites.Data[index];
                }
            }

            m_reorderableList.list = assetDatas;

            m_reorderableList.drawElementCallback = DrawElement;
            m_reorderableList.onChangedCallback = Changed;
            m_reorderableList.drawHeaderCallback = DrawHeader;
            m_reorderableList.drawFooterCallback = DrawFooter;
            m_reorderableList.drawNoneElementCallback = NoneElement;

            m_reorderableList.headerHeight = 0;
            m_reorderableList.footerHeight = 0;
            m_reorderableList.elementHeight = CONST.GUI_LAYOUT_HEIGHT;
            m_reorderableList.showDefaultBackground = false;
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        private void DragAndDropGUI()
        {
            GUI.Box(GUILayoutUtility.GetRect(0, CONST.GUI_LAYOUT_HEIGHT, GUILayout.ExpandWidth(true)),
                LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.DragAndDrop), m_boxStyle.Inst);

            if (!GetObjects(out List<UnityEngine.Object> objs, m_pipeline.Root))
            {
                return;
            }

            foreach (var item in objs)
            {
                AddAssetToObject(item);
            }

            m_pipeline.Favorites.SaveFavoritesData();
        }

        /// <summary>
        /// アセット表示
        /// </summary>
        private void DrawAssetGUI()
        {
            m_scrollVec = GUILayout.BeginScrollView(m_scrollVec);
            m_reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// お気に入りの描画
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool DrawAsset(Rect rect, int index, bool isActive, bool isFocused)
        {
            float BUTTON_WIDTH = 30;
            var assetData = m_pipeline.Favorites.Data[index];
            var copyRect = rect;

            copyRect.width = BUTTON_WIDTH;

            // Ping を実行
            AssetDrawer.OnPingObjectButton(copyRect, assetData);

            rect.x += BUTTON_WIDTH;
            rect.width -= BUTTON_WIDTH * 2;

            // アセットを開くボタン
            AssetDrawer.OnAssetButton(m_pipeline, rect, assetData, OpenAsset);

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
        public void AddAssetToObject(Object assetObject)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assetObject, out string guid, out long localid))
            {
                return;
            }

            // Assetは保存済み
            if (m_pipeline.Favorites.ExistsGUID(guid, localid))
            {
                return;
            }

            // お気に入りに登録
            m_pipeline.Favorites.Add(guid, localid);
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
        private void RemoveAsset(AssetData info)
        {
            m_pipeline.Favorites.Remove(info);
            m_pipeline.Favorites.SaveFavoritesData();
        }

        /// <summary>
        /// アセットを開く
        /// </summary>
        /// <param name="info"></param>
        private void OpenAsset(AssetData info)
        {
            // シーンアセットを開こうとしている
            if (Path.GetExtension(info.Path).Equals(CONST.UNITY_EXT))
            {
                // シーンの保存を促す
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
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
        public static bool GetObjects(out List<UnityEngine.Object> targetList, EditorWindow window, Rect? rect = null)
        {
            targetList = null;

            // ドラックドロップされたオブジェクトがなければ終わり
            var objectReferences = GetObjects(window, rect);
            if (objectReferences == null)
            {
                return false;
            }

            // ドロップされたオブジェクトに対象の物が無ければ終わり
            targetList = objectReferences.ToList();
            if (targetList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ドラッグ&ドロップでオブジェクトを取得
        /// </summary>
        /// <param name="areaTitle"></param>
        /// <param name="widthMin"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static UnityEngine.Object[] GetObjects(EditorWindow window, Rect? rect = null)
        {
            var ev = Event.current;

            // エリアが指定されていれば範囲内か確認
            if (rect.HasValue && !rect.Value.Contains(ev.mousePosition))
            {
                return null;
            }

            // ドラッグ＆ドロップで操作が更新された時でも、実行した時でもなければ終わり
            if (ev.type != EventType.DragUpdated && ev.type != EventType.DragPerform)
            {
                return null;
            }

            var paths = DragAndDrop.paths;
            if (EditorWindow.mouseOverWindow != window || paths.Length <= 0)
            {
                return null;
            }

            // カーソルに「+」のアイコンを表示
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            // ドラッグ&ドロップで無ければ終わり
            if (ev.type != EventType.DragPerform)
            {
                return null;
            }

            //ドラッグを受け付ける
            DragAndDrop.AcceptDrag();

            DragAndDrop.activeControlID = 0;

            //イベントを使用済みにする
            Event.current.Use();

            return DragAndDrop.objectReferences;
        }

        public void Save()
        {
            m_pipeline.Favorites.SaveFavoritesData();
        }
    }
}
#endif

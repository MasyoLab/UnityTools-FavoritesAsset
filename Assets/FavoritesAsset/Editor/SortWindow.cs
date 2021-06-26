using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    /// <summary>
    /// ソート画面
    /// </summary>
    class SortWindow : EditorWindow {
        static SortWindow _inst = null;
        ReorderableList _reorderableList = null;
        List<AssetInfo> _assetInfos = null;
        Vector2 _scrollVec2;

        private void OnGUI() {
            // スクロールビュー
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList?.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public static void Display(EditorWindow win, FavoritesManager manager) {
            _inst?.Close();
            _inst = CreateInstance<SortWindow>();
            _inst.titleContent = new GUIContent(CONST.SORT_WINDOW) {
                image = EditorGUIUtility.IconContent(CONST.SELECTION_LIST_TEMPLATE_ICON).image
            };
            _inst.SetData(win, manager);
            _inst.Show();
        }

        public void SetData(EditorWindow editorWindow, FavoritesManager manager) {
            // データを複製
            _assetInfos = manager.Data.ToList();

            // 入れ替え時に呼び出す
            void OnChanged(ReorderableList list) {
                if (_assetInfos == null)
                    return;
                manager?.SortData(_assetInfos);
                // 描画
                editorWindow?.Repaint();
            }

            _reorderableList = new ReorderableList(_assetInfos, typeof(GameObject)) {
                drawElementCallback = OnDrawElement,
                onChangedCallback = OnChanged
            };
        }

        void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            AssetDrawer.OnAssetButton(rect, _assetInfos[index]);
        }
    }
}
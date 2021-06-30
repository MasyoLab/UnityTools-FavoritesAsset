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
    class SortWindow {
        ReorderableList _reorderableList = null;
        List<AssetInfo> _assetInfos = null;
        Vector2 _scrollVec2;

        public SortWindow() { }
        public SortWindow(FavoritesManager manager) {
            SetData(manager);
        }

        public void SortGUI() {
            // スクロールビュー
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList?.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public void SetData(FavoritesManager manager) {
            // データを複製
            _assetInfos = manager.Data.ToList();

            // 入れ替え時に呼び出す
            void OnChanged(ReorderableList list) {
                if (_assetInfos == null)
                    return;
                manager?.SortData(_assetInfos);
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
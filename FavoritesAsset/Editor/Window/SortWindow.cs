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
    class SortWindow : BaseWindow {
        ReorderableList _reorderableList = null;
        List<AssetInfo> _assetInfos = null;
        Vector2 _scrollVec2;

        public override void OnGUI(Rect windowSize) {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList?.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public override void Init(FavoritesManager manager, EditorWindow root) {
            base.Init(manager, root);

            // データを複製
            _assetInfos = _manager.Data.ToList();

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list) {
                if (_assetInfos == null)
                    return;
                _manager.SortData(_assetInfos);
            }

            _reorderableList = new ReorderableList(_assetInfos, typeof(GameObject)) {
                drawElementCallback = DrawElement,
                onChangedCallback = Changed,
                drawHeaderCallback = DrawHeader,
                drawFooterCallback = DrawFooter,
            };
        }

        void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            AssetDrawer.OnAssetButton(rect, _assetInfos[index]);
        }

        void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "");
        }

        private void DrawFooter(Rect rect) {
            EditorGUI.LabelField(rect, "");
        }
    }
}
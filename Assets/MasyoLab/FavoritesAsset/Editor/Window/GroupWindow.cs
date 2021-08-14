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

    class GroupWindow : BaseWindow {

        ReorderableList _reorderableList = null;
        Vector2 _scrollVec2;
        bool _isUpdate = false;

        PtrLinker<GUIStyle> _textArea = new PtrLinker<GUIStyle>(() => {
            return new GUIStyle(EditorStyles.textArea) {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };
        });

        public override void Init(PtrLinker<SystemManager> manager, EditorWindow root) {
            base.Init(manager, root);
            InitGroupGUI();
            _isUpdate = false;
        }

        public override void OnGUI(Rect windowSize) {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public override void Close() {
            base.Close();
            if (_isUpdate == false)
                return;
            _group.Save();
        }

        void InitGroupGUI() {
            if (_reorderableList != null)
                return;

            // データを複製
            var groupDatas = _group.GroupDB.Data.ToList();

            void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, "グループリスト");
                if (groupDatas.Count != _group.GroupDB.Data.Count) {
                    groupDatas = _group.GroupDB.Data.ToList();
                    _reorderableList.list = groupDatas;
                }
            }

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list) {
                for (int i = 0; i < groupDatas.Count; i++) {
                    groupDatas[i].Index = i;
                }
                _group.Sort();
                _isUpdate = true;
            }

            void AddCallback(ReorderableList list) {
                groupDatas.Add(_group.AddData());
                _group.Save();
            }

            void RemoveCallback(ReorderableList list) {
                groupDatas.RemoveAt(list.index);
                _group.Remove(list.index);
                _group.Save();
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                var item = groupDatas[index];
                rect.height -= 2;
                rect.y += 1;
                var newText = GUI.TextArea(rect, item.GroupName, _textArea.Inst);
                if (newText != item.GroupName) {
                    item.GroupName = newText;
                    _group.UpdateGroupStr();
                    _isUpdate = true;
                }
            }

            void NoneElement(Rect rect) {
                EditorGUI.LabelField(rect, "お気に入りグループがありません");
            }

            _reorderableList = new ReorderableList(groupDatas, typeof(AssetInfo)) {
                drawElementCallback = DrawElement,
                onChangedCallback = Changed,
                drawHeaderCallback = DrawHeader,
                onAddCallback = AddCallback,
                onRemoveCallback = RemoveCallback,
                drawNoneElementCallback = NoneElement,
            };
        }
    }
}
#if UNITY_EDITOR
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

        public override void Init(IPipeline pipeline) {
            base.Init(pipeline);
            InitGroupGUI();
            _isUpdate = false;
        }

        public override void OnGUI() {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public override void Close() {
            base.Close();
            if (_isUpdate == false)
                return;
            _pipeline.Group.Save();
        }

        public override void Reload() {
            base.Reload();
            InitGroupGUI();
        }

        void InitGroupGUI() {

            // データを複製
            var groupDatas = _pipeline.Group.GroupDB.Data.ToList();

            if (_reorderableList == null) {
                _reorderableList = new ReorderableList(groupDatas, typeof(AssetData));
            }

            void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, LanguageData.GetText(_pipeline.Setting.Language, TextEnum.FavoriteGroup));
                if (groupDatas.Count != _pipeline.Group.GroupDB.Data.Count) {
                    groupDatas = _pipeline.Group.GroupDB.Data.ToList();
                    _reorderableList.list = groupDatas;
                }
            }

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list) {
                for (int i = 0; i < groupDatas.Count; i++) {
                    groupDatas[i].Index = i;
                }
                _pipeline.Group.Sort();
                _isUpdate = true;
            }

            void AddCallback(ReorderableList list) {
                groupDatas.Add(_pipeline.Group.AddData());
                _pipeline.Group.Save();
            }

            void RemoveCallback(ReorderableList list) {
                groupDatas.RemoveAt(list.index);
                _pipeline.Group.Remove(list.index);
                _pipeline.Group.Save();
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                var item = groupDatas[index];
                rect.height -= 2;
                rect.y += 1;
                var newText = EditorGUI.TextField(rect, item.GroupName);
                if (newText != item.GroupName) {
                    item.GroupName = newText;
                    _pipeline.Group.UpdateGroupNameList();
                    _isUpdate = true;
                }
            }

            void NoneElement(Rect rect) {
                EditorGUI.LabelField(rect, LanguageData.GetText(_pipeline.Setting.Language, TextEnum.FavoriteGroupIsEmpty));
            }

            _reorderableList.list = groupDatas;
            _reorderableList.drawElementCallback = DrawElement;
            _reorderableList.onChangedCallback = Changed;
            _reorderableList.drawHeaderCallback = DrawHeader;
            _reorderableList.onAddCallback = AddCallback;
            _reorderableList.onRemoveCallback = RemoveCallback;
            _reorderableList.drawNoneElementCallback = NoneElement;
        }
    }
}
#endif

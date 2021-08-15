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

        public override void Reload() {
            base.Reload();
            InitGroupGUI();
        }

        void InitGroupGUI() {
            
            // データを複製
            var groupDatas = _group.GroupDB.Data.ToList();

            if (_reorderableList == null) {
                _reorderableList = new ReorderableList(groupDatas, typeof(AssetData));
            }

            void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, LanguageData.GetText(_setting.Language, TextEnum.FavoriteGroup));
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
                var newText = EditorGUI.TextArea(rect, item.GroupName);
                if (newText != item.GroupName) {
                    item.GroupName = newText;
                    _group.UpdateGroupNameList();
                    _isUpdate = true;
                }
            }

            void NoneElement(Rect rect) {
                EditorGUI.LabelField(rect, LanguageData.GetText(_setting.Language, TextEnum.FavoriteGroupIsEmpty));
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
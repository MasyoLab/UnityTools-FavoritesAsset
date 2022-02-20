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

        private ReorderableList m_reorderableList = null;
        private Vector2 m_scrollVec2 = Vector2.zero;
        private bool m_isUpdate = false;

        public override void Init(IPipeline pipeline) {
            base.Init(pipeline);
            InitGroupGUI();
            m_isUpdate = false;
        }

        public override void OnGUI() {
            m_scrollVec2 = GUILayout.BeginScrollView(m_scrollVec2);
            m_reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public override void Close() {
            base.Close();
            if (m_isUpdate == false) {
                return;
            }
            m_pipeline.Group.Save();
        }

        public override void Reload() {
            base.Reload();
            InitGroupGUI();
        }

        private void InitGroupGUI() {

            // データを複製
            var groupDatas = m_pipeline.Group.GroupDB.Data.ToList();

            if (m_reorderableList == null) {
                m_reorderableList = new ReorderableList(groupDatas, typeof(AssetData));
            }

            void DrawHeader(Rect rect) {
                EditorGUI.LabelField(rect, LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.FavoriteGroup));
                if (groupDatas.Count != m_pipeline.Group.GroupDB.Data.Count) {
                    groupDatas = m_pipeline.Group.GroupDB.Data.ToList();
                    m_reorderableList.list = groupDatas;
                }
            }

            // 入れ替え時に呼び出す
            void Changed(ReorderableList list) {
                for (int i = 0; i < groupDatas.Count; i++) {
                    groupDatas[i].Index = i;
                }
                m_pipeline.Group.Sort();
                m_isUpdate = true;
            }

            void AddCallback(ReorderableList list) {
                groupDatas.Add(m_pipeline.Group.AddData());
                m_pipeline.Group.Save();
            }

            void RemoveCallback(ReorderableList list) {
                groupDatas.RemoveAt(list.index);
                m_pipeline.Group.Remove(list.index);
                m_pipeline.Group.Save();
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
                var item = groupDatas[index];
                rect.height -= 2;
                rect.y += 1;
                var newText = EditorGUI.TextField(rect, item.GroupName);
                if (newText != item.GroupName) {
                    item.GroupName = newText;
                    m_pipeline.Group.UpdateGroupNameList();
                    m_isUpdate = true;
                }
            }

            void NoneElement(Rect rect) {
                EditorGUI.LabelField(rect, LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.FavoriteGroupIsEmpty));
            }

            m_reorderableList.list = groupDatas;
            m_reorderableList.drawElementCallback = DrawElement;
            m_reorderableList.onChangedCallback = Changed;
            m_reorderableList.drawHeaderCallback = DrawHeader;
            m_reorderableList.onAddCallback = AddCallback;
            m_reorderableList.onRemoveCallback = RemoveCallback;
            m_reorderableList.drawNoneElementCallback = NoneElement;
        }
    }
}
#endif

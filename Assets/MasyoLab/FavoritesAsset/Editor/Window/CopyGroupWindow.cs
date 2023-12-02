#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace MasyoLab.Editor.FavoritesAsset
{
    class CopyGroupWindow : BaseWindow
    {
        private PtrLinker<GUIStyle> m_labelStyle = new PtrLinker<GUIStyle>(() =>
        {
            return new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
            };
        });

        private int m_index = 0;

        public override void Init(IPipeline pipeline)
        {
            base.Init(pipeline);
        }

        public override void OnGUI()
        {
            GUILayout.Label(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.CopyFavoriteGroupFeatureDescription), m_labelStyle.Inst);
            Utils.GUILine();
            m_index = EditorGUILayout.Popup(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.CopyFavoriteGroupPulldownDescription), m_index, m_pipeline.Group.GroupNames);

            var content = new GUIContent(LanguageData.GetText(m_pipeline.Setting.Language, TextEnum.CopyFavoriteGroupReplicationButton));
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                var selectIndex = m_index;
                var baseGroup = m_pipeline.Group.GetData(selectIndex);
                var newGroup = m_pipeline.Group.AddData();
                newGroup.GroupName = $"{baseGroup.GroupName}(Copy)";
                m_pipeline.Group.Save();
                m_pipeline.Group.UpdateGroupNameList();
                m_pipeline.Favorites.ReplicationFavoriteData(baseGroup.GUID, newGroup.GUID);
                m_pipeline.Favorites.SaveFavoritesData();
            }
        }

        public override void Close()
        {
            base.Close();
        }

        public override void Reload()
        {
            base.Reload();
            InitGroupGUI();
        }

        private void InitGroupGUI()
        {
            m_index = m_pipeline.Group.Index;
        }
    }
}
#endif

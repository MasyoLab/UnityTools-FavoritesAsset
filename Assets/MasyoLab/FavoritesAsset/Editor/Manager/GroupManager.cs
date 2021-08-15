using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    /// <summary>
    /// グループ選択時のイベント
    /// </summary>
    enum GroupSelectEventEnum {
        Unselect,
        Select,
        Open,
    }

    class GroupManager {
        PtrLinker<GroupDB> _groupDB = new PtrLinker<GroupDB>(Load);
        public GroupDB GroupDB => _groupDB.Inst;

        /// <summary>
        /// グループ名リスト
        /// </summary>
        List<string> _groupNameList = null;
        /// <summary>
        /// グループ名リスト(中継)
        /// </summary>
        string[] _groupNames = null;
        /// <summary>
        /// グループ名
        /// </summary>
        public string[] GroupNames => GetGroupName();

        int _index = -1;
        /// <summary>
        /// 選択中のグループID
        /// </summary>
        public int Index {
            private set => _index = value;
            get {
                if (_index == -1) {
                    for (int i = 0; i < GroupDB.Data.Count; i++) {
                        GroupDB.Data[i].Index = i;
                    }
                    SelectGroupByGUID();
                }
                return _index;
            }
        }

        /// <summary>
        /// 現在選択しているグループの保存ファイル
        /// </summary>
        public string SelectGroupFileName => GroupDB.SelectGroupGUID == string.Empty ? CONST.FAVORITES_DATA : GroupDB.SelectGroupGUID;

        /// <summary>
        /// グループ破棄イベント
        /// </summary>
        public UnityAction<string> RemoveEvent;

        PtrLinker<SettingManager> _settingManager = null;

        public void Save() {
            SaveLoad.Save(JsonUtility.ToJson(GroupDB), SaveLoad.GetSaveDataPath(CONST.GROUP_DATA));
        }

        static GroupDB Load() {
            string jsonData = SaveLoad.Load(SaveLoad.GetSaveDataPath(CONST.GROUP_DATA));

            // json から読み込む
            var assets = JsonUtility.FromJson<GroupDB>(jsonData);
            if (assets == null) {
                return new GroupDB();
            }
            return assets;
        }

        /// <summary>
        /// インポートしたデータを登録
        /// </summary>
        /// <param name="importData"></param>
        public void SetImportData(FavoritesJsonExportData importData) {
            if (importData == null)
                return;

            GroupDB.Set(importData.GroupDB);
            SelectGroupByGUID();
            UpdateGroupNameList();
            Save();
        }

        /// <summary>
        /// グループ削除
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index) {
            var data = GroupDB.Data[index];
            var guid = data.GUID;

            data.GroupName = string.Empty;

            // 一度使ったグループGUIDを予約済みにする
            GroupDB.Reserved.Add(data);
            GroupDB.Data.RemoveAt(index);

            SelectGroupByGUID();
            UpdateGroupNameList();
            RemoveEvent?.Invoke(guid);
        }

        /// <summary>
        /// グループ追加
        /// </summary>
        /// <returns></returns>
        public GroupData AddData() {
            // 予約済みリストから取得
            if (GroupDB.Reserved.Count != 0) {
                var data = GroupDB.Reserved[0];
                GroupDB.Data.Add(data);
                GroupDB.Reserved.RemoveAt(0);
                UpdateGroupNameList();
                return data;
            }
            // 新規作成
            else {
                var data = new GroupData();
                GroupDB.Data.Add(data);
                UpdateGroupNameList();
                return data;
            }
        }

        /// <summary>
        /// ソート
        /// </summary>
        public void Sort() {
            SelectGroupByGUID(true);
            UpdateGroupNameList();
        }

        /// <summary>
        /// グループ名
        /// </summary>
        /// <returns></returns>
        string[] GetGroupName() {
            if (_groupNames != null)
                return _groupNames;
            return UpdateGroupNameList();
        }

        /// <summary>
        /// グループ名リストを更新
        /// </summary>
        /// <returns></returns>
        public string[] UpdateGroupNameList() {
            if (_groupNameList == null) {
                _groupNameList = new List<string>();
            }

            int index = 0;

            _groupNameList.Clear();
            _groupNameList.Add($"{index}: {CONST.DEFAULT}");
            index++;

            foreach (var item in GroupDB.Data) {
                if (item.IsNull)
                    continue;
                _groupNameList.Add($"{index}: {item.GroupName}");
                index++;
            }

            _groupNameList.Add("");
            _groupNameList.Add(LanguageData.GetText(_settingManager.Inst.Language, TextEnum.AddNewFavoriteGroup));

            return _groupNames = _groupNameList.ToArray();
        }

        /// <summary>
        /// グループを切り替える
        /// </summary>
        /// <param name="selectIndex"></param>
        /// <returns></returns>
        public GroupSelectEventEnum SelectGroupByIndex(int selectIndex) {
            var isSave = selectIndex != Index;

            if (selectIndex == 0) {
                GroupDB.SelectGroupGUID = string.Empty;
                Index = selectIndex;
            }
            else if (selectIndex == GroupNames.Length - 1) {
                return GroupSelectEventEnum.Open;
            }
            else {
                GroupDB.SelectGroupGUID = GroupDB.Data[selectIndex - 1].GUID;
                Index = selectIndex;
            }

            if (isSave) {
                Save();
                return GroupSelectEventEnum.Select;
            }

            return GroupSelectEventEnum.Unselect;
        }

        /// <summary>
        /// GUIDで選択中のグループを切り替える
        /// </summary>
        /// <param name="isSort"></param>
        void SelectGroupByGUID(bool isSort = false) {
            var groupData = GroupDB.Data.Find(v => v.GUID == GroupDB.SelectGroupGUID);

            if (isSort) {
                GroupDB.Data.Sort((itemA, itemB) => itemA.Index - itemB.Index);
                for (int i = 0; i < GroupDB.Data.Count; i++) {
                    GroupDB.Data[i].Index = i;
                }
            }

            if (groupData == null) {
                GroupDB.SelectGroupGUID = string.Empty;
                Index = 0;
                return;
            }

            Index = groupData.Index + 1;
        }

        /// <summary>
        /// グループ名を取得
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public string GetGroupNameByGUID(string guid) {
            var groupData = GroupDB.Data.Find(v => v.GUID == guid);
            if (groupData == null) {
                return CONST.DEFAULT;
            }
            return groupData.GroupName;
        }

        public void SetSettingManager(PtrLinker<SettingManager> settingManager) {
            _settingManager = settingManager;
        }
    }
}

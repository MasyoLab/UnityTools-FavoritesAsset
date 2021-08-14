﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {
    class GroupManager {
        PtrLinker<GroupDB> _groupDB = new PtrLinker<GroupDB>(Load);
        public GroupDB GroupDB => _groupDB.Inst;

        List<string> _groupStrList = null;
        string[] _groupStr = null;
        public string[] GroupStr => GetGroupStr();

        int _index = 0;
        public int Index => _index;

        public string SelectGroupFileName => GroupDB.SelectGroupGUID == string.Empty ? CONST.FAVORITES_DATA : GroupDB.SelectGroupGUID;

        public void Save() {
            SaveLoad.Save(JsonUtility.ToJson(_groupDB.Inst), SaveLoad.GetSaveDataPath(CONST.GROUP_DATA));
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

        public void SetImportData(FavoritesJsonExportData importData) {
            if (importData == null)
                return;

            GroupDB.Set(importData.GroupDB);
            SelectGroupByGUID();
            UpdateGroupStr();
            Save();
        }

        public void Remove(int index) {
            var data = GroupDB.Data[index];
            data.GroupName = string.Empty;
            GroupDB.Reserved.Add(data);
            GroupDB.Data.RemoveAt(index);
            SelectGroupByGUID();
            UpdateGroupStr();
        }

        public GroupData AddData() {
            if (GroupDB.Reserved.Count != 0) {
                var data = GroupDB.Reserved[0];
                GroupDB.Data.Add(data);
                GroupDB.Reserved.RemoveAt(0);
                UpdateGroupStr();
                return data;
            }
            else {
                var data = new GroupData();
                GroupDB.Data.Add(data);
                UpdateGroupStr();
                return data;
            }
        }

        public void Sort() {
            SelectGroupByGUID(true);
            UpdateGroupStr();
        }

        string[] GetGroupStr() {
            if (_groupStr != null)
                return _groupStr;

            return UpdateGroupStr();
        }

        public string[] UpdateGroupStr() {
            if (_groupStrList == null) {
                _groupStrList = new List<string>();
            }

            int index = 0;

            _groupStrList.Clear();
            _groupStrList.Add($"{index}: {CONST.DEFAULT}");
            index++;

            foreach (var item in GroupDB.Data) {
                if (item.IsNull)
                    continue;
                _groupStrList.Add($"{index}: {item.GroupName}");
                index++;
            }

            _groupStrList.Add("");
            _groupStrList.Add("Add New FavoriteGroup ...");

            return _groupStr = _groupStrList.ToArray();
        }

        public bool SelectGroupGUI() {
            var selectIndex = EditorGUILayout.Popup(_index, GroupStr);

            if (selectIndex == 0) {
                GroupDB.SelectGroupGUID = string.Empty;
                _index = selectIndex;
            }
            else if (selectIndex == GroupStr.Length - 1) {
                return true;
            }
            else {
                GroupDB.SelectGroupGUID = GroupDB.Data[selectIndex - 1].GUID;
                _index = selectIndex;
            }
            return false;
        }

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
                _index = 0;
                return;
            }

            _index = groupData.Index + 1;
        }
    }
}

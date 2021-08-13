using System.Collections;
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

        public void Remove(int index) {
            var data = GroupDB.Data[index];
            data.GroupName = string.Empty;
            GroupDB.Reserved.Add(data);
            GroupDB.Data.RemoveAt(index);
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
            GroupDB.Data.Sort((itemA, itemB) => itemA.Index - itemB.Index);
            foreach (var item in GroupDB.Data) {
                item.Index = 0;
            }
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
    }
}

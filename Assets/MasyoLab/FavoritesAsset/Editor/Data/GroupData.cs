using System.Collections;
using System.Collections.Generic;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    [System.Serializable]
    class GroupData {
        public string GroupName = string.Empty;
        public string GUID = Utils.NewGuid();
        public bool IsNull => string.IsNullOrWhiteSpace(GroupName);
        [System.NonSerialized]
        public int Index = 0;
    }

    [System.Serializable]
    class GroupDB {
        public string SelectGroupGUID = string.Empty;
        public List<GroupData> Data = new List<GroupData>();
        public List<GroupData> Reserved = new List<GroupData>();

        public void Set(GroupDB groupDB) {
            SelectGroupGUID = groupDB.SelectGroupGUID;
            Data = groupDB.Data;
            Reserved = groupDB.Reserved;
        }
    }
}
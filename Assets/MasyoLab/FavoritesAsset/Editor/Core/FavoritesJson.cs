using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class FavoritesJson {
        public AssetInfoList AssetDB;

        public static string ToJson(AssetInfoList assetInfo) {
            return JsonUtility.ToJson(new FavoritesJson {
                AssetDB = assetInfo,
            });
        }

        public static FavoritesJson FromJson(string jsonData) {
            return JsonUtility.FromJson<FavoritesJson>(jsonData);
        }
    }

    class FavoritesJsonExportData {
        public AssetInfoList AssetDB;
        public List<AssetInfoList> GroupData;
        public GroupDB GroupDB;

        public static string ToJson(AssetInfoList assetInfo, GroupDB groupDB, params AssetInfoList[] group) {
            var data = new FavoritesJsonExportData();
            data.AssetDB = assetInfo;
            data.GroupData = new List<AssetInfoList>();
            if (group != null) {
                data.GroupData.AddRange(group);
            }
            data.GroupDB = groupDB;
            return JsonUtility.ToJson(data);
        }

        public static FavoritesJsonExportData FromJson(string jsonData) {
            return JsonUtility.FromJson<FavoritesJsonExportData>(jsonData);
        }
    }
}

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
        public AssetDB AssetDB;

        public static string ToJson(AssetDB assetDB) {
            return JsonUtility.ToJson(new FavoritesJson {
                AssetDB = assetDB,
            });
        }

        public static FavoritesJson FromJson(string jsonData) {
            return JsonUtility.FromJson<FavoritesJson>(jsonData);
        }
    }

    class FavoritesJsonExportData {
        public AssetDB AssetDB;
        public List<AssetDB> GroupData;
        public GroupDB GroupDB;

        public static string ToJson(AssetDB assetDB, GroupDB groupDB, List<AssetDB> assetDBList) {
            var data = new FavoritesJsonExportData();
            data.AssetDB = assetDB;
            data.GroupData = assetDBList;
            data.GroupDB = groupDB;
            return JsonUtility.ToJson(data);
        }

        public static FavoritesJsonExportData FromJson(string jsonData) {
            return JsonUtility.FromJson<FavoritesJsonExportData>(jsonData);
        }
    }
}

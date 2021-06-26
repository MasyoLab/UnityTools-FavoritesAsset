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
}
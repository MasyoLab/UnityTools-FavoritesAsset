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

    /// <summary>
    /// アセットリスト
    /// </summary>
    [System.Serializable]
    class AssetInfo {
        /// <summary>
        /// アセットのGUID
        /// </summary>
        public string Guid = string.Empty;
        /// <summary>
        /// アセットパス
        /// </summary>
        public string Path = string.Empty;
        /// <summary>
        /// アセット名
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// アセットタイプ
        /// </summary>
        public string Type = string.Empty;

        public AssetInfo() { }
        public AssetInfo(string guid, string path, string name, string type) {
            Guid = guid;
            Path = path;
            Name = name;
            Type = type;
        }
    }

    [System.Serializable]
    class AssetInfoList {
        public LanguageEnum Language = LanguageEnum.English;
        public List<AssetInfo> Ref = new List<AssetInfo>();
    }
}
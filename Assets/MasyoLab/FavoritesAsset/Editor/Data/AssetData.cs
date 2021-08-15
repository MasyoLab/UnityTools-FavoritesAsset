using System.Collections;
using System.Collections.Generic;

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
    class AssetData {
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

        [System.NonSerialized]
        public int Index = 0;

        public AssetData() { }
        public AssetData(string guid, string path, string name, string type) {
            Guid = guid;
            Path = path;
            Name = name;
            Type = type;
        }
    }

    [System.Serializable]
    class AssetDB {
        public string Guid = string.Empty;
        public List<AssetData> Ref = new List<AssetData>();
    }
}

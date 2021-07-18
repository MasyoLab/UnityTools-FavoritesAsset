using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    /// <summary>
    /// お気に入りマネージャー
    /// </summary>
    class FavoritesManager {
        PtrLinker<AssetInfoList> _assetInfo = new PtrLinker<AssetInfoList>(LoadFavoritesData);
        PtrLinker<SettingManager> _setting = new PtrLinker<SettingManager>();

        /// <summary>
        /// データ
        /// </summary>
        public IReadOnlyList<AssetInfo> Data => _assetInfo.Inst.Ref;

        public LanguageEnum Language {
            get => _setting.Inst.Language;
            set => _setting.Inst.Language = value;
        }

        public string ImportTarget {
            get => _setting.Inst.ImportTarget;
            set => _setting.Inst.ImportTarget = value;
        }

        public string ExportTarget {
            get => _setting.Inst.ExportTarget;
            set => _setting.Inst.ExportTarget = value;
        }

        public string AssetDBJson => FavoritesJson.ToJson(_assetInfo.Inst);

        public void Add(AssetInfo info) => _assetInfo.Inst.Ref.Add(info);

        public void Add(string guid, string path, string name, string type) {
            _assetInfo.Inst.Ref.Add(new AssetInfo(guid, path, name, type));
        }

        public void Remove(AssetInfo info) => _assetInfo.Inst.Ref.Remove(info);

        public void RemoveAll() => _assetInfo.Inst.Ref.RemoveRange(0, _assetInfo.Inst.Ref.Count);

        public bool ExistsGUID(string guid) => _assetInfo.Inst.Ref.Exists(x => x.Guid == guid);

        public bool ExistsAssetPath(string path) => _assetInfo.Inst.Ref.Exists(x => x.Path == path);

        public void SaveFavoritesData() {
            SaveLoad.Save(AssetDBJson, SaveLoad.GetSaveDataPath(CONST.FAVORITES_DATA));
        }

        static AssetInfoList LoadFavoritesData() {
            string jsonData = SaveLoad.Load(SaveLoad.GetSaveDataPath(CONST.FAVORITES_DATA));

            // json から読み込む
            var assets = JsonUtility.FromJson<AssetInfoList>(jsonData);
            if (assets == null) {
                return new AssetInfoList();
            }
            return FavoritesJson.FromJson(jsonData).AssetDB;
        }

        /// <summary>
        /// お気に入り登録したアセットを更新
        /// </summary>
        public void CheckFavoritesAsset() {
            bool isUpdate = false;

            foreach (var item in Data) {
                // GUIDでパスを取得
                var newPath = AssetDatabase.GUIDToAssetPath(item.Guid);
                if (newPath == string.Empty)
                    continue;

                item.Path = newPath;

                // アセットの情報
                var assetData = AssetDatabase.LoadAssetAtPath<Object>(item.Path);
                if (assetData == null)
                    continue;

                item.Name = assetData.name;
                item.Type = assetData.GetType().ToString();

                isUpdate = isUpdate ? isUpdate : item.Path != newPath;
            }

            if (isUpdate) {
                SaveFavoritesData();
            }
        }

        /// <summary>
        /// ソートデータを受け取る
        /// </summary>
        /// <param name="assetInfos"></param>
        public void SortData(in List<AssetInfo> assetInfos) {
            var newData = new List<AssetInfo>(_assetInfo.Inst.Ref.Count);

            foreach (var item in assetInfos) {
                var outItem = _assetInfo.Inst.Ref.Find(data => data.Guid == item.Guid);
                if (outItem == null)
                    continue;
                newData.Add(outItem);
            }

            _assetInfo.Inst.Ref.Clear();
            _assetInfo.Inst.Ref.AddRange(newData);
            SaveFavoritesData();
        }

        public void SetJsonData(string jsonData) {
            if (jsonData == string.Empty)
                return;
            _assetInfo.SetInst(FavoritesJson.FromJson(jsonData).AssetDB);
            SaveFavoritesData();
        }
    }
}
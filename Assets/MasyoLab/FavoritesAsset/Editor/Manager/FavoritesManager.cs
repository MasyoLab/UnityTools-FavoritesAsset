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

        PtrLinker<GroupManager> _groupManager = null;
        Dictionary<string, PtrLinker<AssetInfoList>> _assetInfoDict = new Dictionary<string, PtrLinker<AssetInfoList>>();
        PtrLinker<AssetInfoList> _assetInfo => SelectFavoritesGroup();

        //PtrLinker<AssetInfoList> _assetInfo = new PtrLinker<AssetInfoList>(LoadFavoritesData);
        public IReadOnlyList<AssetInfo> Data => _assetInfo.Inst.Ref;
        public AssetInfoList AssetInfoList => SelectFavoritesGroup(CONST.FAVORITES_DATA).Inst;

        public void Add(AssetInfo info) => _assetInfo.Inst.Ref.Add(info);

        public void Add(string guid, string path, string name, string type) {
            _assetInfo.Inst.Ref.Add(new AssetInfo(guid, path, name, type));
        }

        public void Remove(AssetInfo info) => _assetInfo.Inst.Ref.Remove(info);

        public void RemoveAll() => _assetInfo.Inst.Ref.RemoveRange(0, _assetInfo.Inst.Ref.Count);

        public bool ExistsGUID(string guid) => _assetInfo.Inst.Ref.Exists(x => x.Guid == guid);

        public bool ExistsAssetPath(string path) => _assetInfo.Inst.Ref.Exists(x => x.Path == path);

        public void SaveFavoritesData() {
            //SaveLoad.Save(FavoritesJson.ToJson(_assetInfo.Inst), SaveLoad.GetSaveDataPath(CONST.FAVORITES_DATA));
            SaveLoad.Save(FavoritesJson.ToJson(_assetInfo.Inst), SaveLoad.GetSaveDataPath(_groupManager.Inst.SelectGroupFileName));
        }

        static AssetInfoList LoadFavoritesData() {
            return LoadFavoritesData(CONST.FAVORITES_DATA);
        }
        static AssetInfoList LoadFavoritesData(string fileName) {
            string jsonData = SaveLoad.Load(SaveLoad.GetSaveDataPath(fileName));
            AssetInfoList data;
            string guid = fileName == CONST.FAVORITES_DATA ? string.Empty : fileName;

            // json から読み込む
            var assets = JsonUtility.FromJson<AssetInfoList>(jsonData);
            if (assets == null) {
                data = new AssetInfoList();
                data.Guid = guid;
                return data;
            }

            data = FavoritesJson.FromJson(jsonData).AssetDB;
            data.Guid = guid;
            return data;
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
        public void SortData(IReadOnlyList<AssetInfo> assetInfos) {
            var newData = new List<AssetInfo>(assetInfos.Count);

            foreach (var item in assetInfos) {
                newData.Add(item);
            }

            _assetInfo.Inst.Ref.Clear();
            _assetInfo.Inst.Ref.AddRange(newData);
            SaveFavoritesData();
        }

        public void SetImportData(FavoritesJsonExportData importData) {
            if (importData == null)
                return;

            //_assetInfo.SetInst(importData.AssetDB);
            //SaveFavoritesData();

            _assetInfoDict.Clear();
            _assetInfoDict.Add(CONST.FAVORITES_DATA, new PtrLinker<AssetInfoList>(() => {
                return importData.AssetDB;
            }));
            foreach (var item in importData.GroupData) {
                _assetInfoDict.Add(item.Guid, new PtrLinker<AssetInfoList>(() => {
                    return item;
                }));
            }

            foreach (var item in _assetInfoDict) {
                SaveLoad.Save(FavoritesJson.ToJson(item.Value.Inst), SaveLoad.GetSaveDataPath(item.Key));
            }
        }

        public void SetGroupManager(PtrLinker<GroupManager> groupManager) {
            _groupManager = groupManager;
            _groupManager.Inst.RemoveEvent = (guid) => {
                _assetInfoDict.Remove(guid);
                SaveLoad.Save("{}", SaveLoad.GetSaveDataPath(guid));
            };
        }

        public PtrLinker<AssetInfoList> SelectFavoritesGroup(string guid) {
            if (_assetInfoDict.ContainsKey(guid)) {
                return _assetInfoDict[guid];
            }

            var favData = new PtrLinker<AssetInfoList>(() => {
                return LoadFavoritesData(guid);
            });
            _assetInfoDict.Add(guid, favData);
            return favData;
        }

        public PtrLinker<AssetInfoList> SelectFavoritesGroup() {
            return SelectFavoritesGroup(_groupManager.Inst.SelectGroupFileName);
        }

        public List<AssetInfoList> GetFavoriteGroups() {
            var returnData = new List<AssetInfoList>(_groupManager.Inst.GroupDB.Data.Count);
            foreach (var item in _groupManager.Inst.GroupDB.Data) {
                returnData.Add(SelectFavoritesGroup(item.GUID).Inst);
            }
            return returnData;
        }
    }
}

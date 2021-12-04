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
    class FavoritesManager : BaseManager {

        Dictionary<string, PtrLinker<AssetDB>> _assetDBDict = new Dictionary<string, PtrLinker<AssetDB>>();
        PtrLinker<AssetDB> _assetDB => SelectFavoritesData();

        //PtrLinker<AssetDB> _assetDB = new PtrLinker<AssetDB>(LoadFavoritesData);
        public IReadOnlyList<AssetData> Data => _assetDB.Inst.Ref;
        public AssetDB AssetInfoList => SelectFavoritesData(CONST.FAVORITES_DATA).Inst;

        public FavoritesManager(IPipeline pipeline) : base(pipeline) {
            // グループ削除時の処理を追加
            pipeline.Group.RemoveEvent = (guid) => {
                _assetDBDict.Remove(guid);
                SaveLoad.Save("{}", SaveLoad.GetSaveDataPath(guid));
            };
        }

        public void Add(AssetData info) => _assetDB.Inst.Ref.Add(info);

        public void Add(string guid, string path, string name, string type) {
            _assetDB.Inst.Ref.Add(new AssetData(guid, path, name, type));
        }

        public void Remove(AssetData info) => _assetDB.Inst.Ref.Remove(info);

        public void RemoveAll() => _assetDB.Inst.Ref.RemoveRange(0, _assetDB.Inst.Ref.Count);

        public bool ExistsGUID(string guid) => _assetDB.Inst.Ref.Exists(x => x.Guid == guid);

        public bool ExistsAssetPath(string path) => _assetDB.Inst.Ref.Exists(x => x.Path == path);

        public void SaveFavoritesData() {
            //SaveLoad.Save(FavoritesJson.ToJson(_assetInfo.Inst), SaveLoad.GetSaveDataPath(CONST.FAVORITES_DATA));
            SaveLoad.Save(FavoritesJson.ToJson(_assetDB.Inst), SaveLoad.GetSaveDataPath(_pipeline.Group.SelectGroupFileName));
        }

        static AssetDB LoadFavoritesData() {
            return LoadFavoritesData(CONST.FAVORITES_DATA);
        }
        static AssetDB LoadFavoritesData(string fileName) {
            string jsonData = SaveLoad.Load(SaveLoad.GetSaveDataPath(fileName));
            AssetDB data;
            string guid = fileName == CONST.FAVORITES_DATA ? string.Empty : fileName;

            // json から読み込む
            var assets = JsonUtility.FromJson<AssetDB>(jsonData);
            if (assets == null) {
                data = new AssetDB();
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
        public void SortData(IReadOnlyList<AssetData> assetInfos) {
            for (int i = 0; i < assetInfos.Count; i++) {
                assetInfos[i].Index = i;
            }
            _assetDB.Inst.Ref.Sort((itemA, itemB) => itemA.Index - itemB.Index);
            SaveFavoritesData();
        }

        /// <summary>
        /// インポート
        /// </summary>
        /// <param name="importData"></param>
        public void SetImportData(FavoritesJsonExportData importData) {
            if (importData == null)
                return;

            //_assetInfo.SetInst(importData.AssetDB);
            //SaveFavoritesData();

            _assetDBDict.Clear();
            _assetDBDict.Add(CONST.FAVORITES_DATA, new PtrLinker<AssetDB>(() => {
                return importData.AssetDB;
            }));
            foreach (var item in importData.GroupData) {
                _assetDBDict.Add(item.Guid, new PtrLinker<AssetDB>(() => {
                    return item;
                }));
            }

            foreach (var item in _assetDBDict) {
                SaveLoad.Save(FavoritesJson.ToJson(item.Value.Inst), SaveLoad.GetSaveDataPath(item.Key));
            }
        }

        /// <summary>
        /// お気に入りデータを切り替える
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        PtrLinker<AssetDB> SelectFavoritesData(string guid) {
            if (_assetDBDict.ContainsKey(guid)) {
                return _assetDBDict[guid];
            }

            var favData = new PtrLinker<AssetDB>(() => {
                return LoadFavoritesData(guid);
            });
            _assetDBDict.Add(guid, favData);
            return favData;
        }

        PtrLinker<AssetDB> SelectFavoritesData() {
            return SelectFavoritesData(_pipeline.Group.SelectGroupFileName);
        }

        public List<AssetDB> GetFavoriteList() {
            var returnData = new List<AssetDB>(_pipeline.Group.GroupDB.Data.Count);
            foreach (var item in _pipeline.Group.GroupDB.Data) {
                returnData.Add(SelectFavoritesData(item.GUID).Inst);
            }
            return returnData;
        }
    }
}

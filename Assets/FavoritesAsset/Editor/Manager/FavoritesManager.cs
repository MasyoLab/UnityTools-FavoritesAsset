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
        /// <summary>
        /// 保存リスト
        /// </summary>
        AssetInfoList _ref = null;
        AssetInfoList _assetInfo {
            get {
                if (_ref == null) {
                    _ref = LoadAssetInfo();
                }
                return _ref;
            }
        }

        SettingManager _refSetting = null;
        SettingManager _setting {
            get {
                if (_refSetting == null) {
                    _refSetting = new SettingManager();
                }
                return _refSetting;
            }
        }

        /// <summary>
        /// データ
        /// </summary>
        public IReadOnlyList<AssetInfo> Data => _assetInfo.Ref;

        public LanguageEnum Language {
            get => _setting.Language;
            set => _setting.Language = value;
        }

        public string AssetDBJson => FavoritesJson.ToJson(_assetInfo);

        public void Add(AssetInfo info) => _assetInfo.Ref.Add(info);

        public void Add(string guid, string path, string name, string type) {
            _assetInfo.Ref.Add(new AssetInfo(guid, path, name, type));
        }

        public void Remove(AssetInfo info) => _assetInfo.Ref.Remove(info);

        public void RemoveAll() => _assetInfo.Ref.RemoveRange(0, _assetInfo.Ref.Count);

        public bool ExistsGUID(string guid) => _assetInfo.Ref.Exists(x => x.Guid == guid);

        public bool ExistsAssetPath(string path) => _assetInfo.Ref.Exists(x => x.Path == path);

        public void SavePrefs() {
            EditorPrefs.SetString(CONST.DATA_KEY_NAME, JsonUtility.ToJson(_assetInfo));
        }

        AssetInfoList LoadAssetInfo() {
            // データがない
            if (!EditorPrefs.HasKey(CONST.DATA_KEY_NAME))
                return new AssetInfoList();

            string jsonData = EditorPrefs.GetString(CONST.DATA_KEY_NAME);

            // json から読み込む
            var assets = JsonUtility.FromJson<AssetInfoList>(jsonData);
            if (assets == null) {
                return new AssetInfoList();
            }
            return assets;
        }

        /// <summary>
        /// お気に入り登録したアセットを更新
        /// </summary>
        public void CheckFavoritesAsset() {
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
            }
            SavePrefs();
        }

        /// <summary>
        /// ソートデータを受け取る
        /// </summary>
        /// <param name="assetInfos"></param>
        public void SortData(in List<AssetInfo> assetInfos) {
            var newData = new List<AssetInfo>(_assetInfo.Ref.Count);

            foreach (var item in assetInfos) {
                var outItem = _assetInfo.Ref.Find(data => data.Guid == item.Guid);
                if (outItem == null)
                    continue;
                newData.Add(outItem);
            }

            _assetInfo.Ref.Clear();
            _assetInfo.Ref.AddRange(newData);
            SavePrefs();
        }

        public void SetJsonData(string jsonData) {
            if (jsonData == string.Empty)
                return;
            _ref = FavoritesJson.FromJson(jsonData).AssetDB;
        }
    }
}
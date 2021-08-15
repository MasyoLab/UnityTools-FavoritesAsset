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

    class SettingManager {

        PtrLinker<SettingData> _settingData = new PtrLinker<SettingData>(LoadSettingData);
        public SettingData Data => _settingData.Inst;

        public LanguageEnum Language {
            get => Data.Language;
            set {
                if (Data.Language != value) {
                    Data.Language = value;
                    SaveSettingData();
                }
            }
        }

        public string ImportTarget {
            get => Data.ImportTarget;
            set {
                if (Data.ImportTarget != value) {
                    Data.ImportTarget = value;
                    SaveSettingData();
                }
            }
        }

        public string ExportTarget {
            get => Data.ExportTarget;
            set {
                if (Data.ExportTarget != value) {
                    Data.ExportTarget = value;
                    SaveSettingData();
                }
            }
        }

        public void SaveSettingData() {
            SaveLoad.Save(JsonUtility.ToJson(Data), SaveLoad.GetSaveDataPath(CONST.SETTING_DATA));
        }

        static SettingData LoadSettingData() {
            string jsonData = SaveLoad.Load(SaveLoad.GetSaveDataPath(CONST.SETTING_DATA));

            // json から読み込む
            var assets = JsonUtility.FromJson<SettingData>(jsonData);
            if (assets == null) {
                return new SettingData();
            }
            return assets;
        }
    }
}

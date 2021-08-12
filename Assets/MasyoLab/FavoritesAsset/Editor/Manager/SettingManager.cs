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

        PtrLinker<SettingData> _data = new PtrLinker<SettingData>(LoadSettingData);

        public SettingData Data => _data.Inst;

        public LanguageEnum Language {
            get => _data.Inst.Language;
            set {
                if (_data.Inst.Language != value) {
                    _data.Inst.Language = value;
                    SaveSettingData();
                }
            }
        }

        public string ImportTarget {
            get => _data.Inst.ImportTarget;
            set {
                if (_data.Inst.ImportTarget != value) {
                    _data.Inst.ImportTarget = value;
                    SaveSettingData();
                }
            }
        }

        public string ExportTarget {
            get => _data.Inst.ExportTarget;
            set {
                if (_data.Inst.ExportTarget != value) {
                    _data.Inst.ExportTarget = value;
                    SaveSettingData();
                }
            }
        }

        public void SaveSettingData() {
            SaveLoad.Save(JsonUtility.ToJson(_data.Inst), SaveLoad.GetSaveDataPath(CONST.SETTING_DATA));
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

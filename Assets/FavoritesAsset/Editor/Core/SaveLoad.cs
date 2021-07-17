using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    struct SaveLoad {
        public static void Save(string jsonData, string filePath) {
            // パス無し
            if (string.IsNullOrEmpty(filePath))
                return;

            // 保存処理
            System.IO.File.WriteAllText(filePath, jsonData);
            AssetDatabase.Refresh();
        }

        public static string Load(string filePath) {
            // パス無し
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            if (!File.Exists(filePath)) {
                return string.Empty;
            }

            var reader = new StreamReader(filePath);
            string jsonStr = reader.ReadLine();
            reader.Close();

            return jsonStr;
        }

        public static void SaveFilePanel(string jsonData) {
            // ファイルパス
            var filePath = EditorUtility.SaveFilePanel(CONST.SAVE, CONST.ASSETS, CONST.JSON_DATA_NAME, CONST.JSON_EXT);
            Save(jsonData, filePath);
        }

        public static string LoadFilePanel() {
            // ファイルパス
            var filePath = EditorUtility.OpenFilePanel(CONST.LOAD, CONST.ASSETS, CONST.JSON_EXT);
            return Load(filePath);
        }

        /// <summary>
        /// 保存先
        /// </summary>
        /// <returns></returns>
        public static string GetSaveDataPath(string fileName, string ext = CONST.JSON_EXT) {
            var filePath = $"{UnityEngine.Application.dataPath.RemoveAtLast(CONST.ASSETS)}{CONST.LIBRARY}/{CONST.FOLDER_NAME}";

            if (!File.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }

            return $"{filePath}/{fileName}.{ext}";
        }
    }
}
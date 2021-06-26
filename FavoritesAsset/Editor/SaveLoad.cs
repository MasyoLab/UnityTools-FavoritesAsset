using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public static void Save(string jsonData) {
            // ファイルパス
            var filePath = EditorUtility.SaveFilePanel(CONST.SAVE, CONST.ASSETS, CONST.JSON_DATA_NAME, CONST.JSON_EXT);

            // パス無し
            if (string.IsNullOrEmpty(filePath))
                return;

            // 保存処理
            System.IO.File.WriteAllText(filePath, jsonData);
            AssetDatabase.Refresh();
        }

        public static string Load() {
            // ファイルパス
            var filePath = EditorUtility.OpenFilePanel(CONST.LOAD, CONST.ASSETS, CONST.JSON_EXT);

            // パス無し
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            var reader = new StreamReader(filePath);
            string jsonStr = reader.ReadLine();
            reader.Close();

            return jsonStr;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
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

            if (!File.Exists(filePath))
                return string.Empty;

            var reader = new StreamReader(filePath);
            string jsonStr = reader.ReadLine();
            reader.Close();

            return jsonStr;
        }

        public static void SaveFile(string jsonData, string directory, UnityEngine.Events.UnityAction<string> result = null) {
            directory = directory == string.Empty ? CONST.ASSETS : directory;

            // ファイルパス
            var filePath = EditorUtility.SaveFilePanel(CONST.SAVE, directory, CONST.JSON_DATA_NAME, CONST.JSON_EXT);
            if (string.IsNullOrEmpty(filePath))
                return;

            result?.Invoke(CreateDirectoryFromFilePath(filePath));
            Save(jsonData, filePath);
        }

        public static string LoadFile(string directory, UnityEngine.Events.UnityAction<string> result = null) {
            directory = directory == string.Empty ? CONST.ASSETS : directory;

            // ファイルパス
            var filePath = EditorUtility.OpenFilePanel(CONST.LOAD, directory, CONST.JSON_EXT);
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            result?.Invoke(CreateDirectoryFromFilePath(filePath));
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

        /// <summary>
        /// ファイルパスからディレクトリを取得
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static string CreateDirectoryFromFilePath(string filePath) {
            var index = filePath.LastIndexOf("/");
            if (index == -1)
                return string.Empty;
            return filePath.RemoveAtLast(filePath.Substring(index));
        }
    }
}
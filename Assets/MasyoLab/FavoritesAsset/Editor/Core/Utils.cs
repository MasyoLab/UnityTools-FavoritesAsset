using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

    struct Utils {
        public static string GetSHA256HashString(string value) {
            SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider();
            return string.Join("", provider.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => $"{x:x2}"));
        }

        public static void MouseCursorLink() {
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
        }

        public static bool OpenURL(string url) {
            try {
                Application.OpenURL(url);
            }
            catch {
                Debug.LogError("Could not open URL. Please check your network connection and ensure the web address is correct.");
                EditorApplication.Beep();
                return false;
            }
            return true;
        }

        public static void GUILine() {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasyoLab.Editor.FavoritesAsset {
    static class StringExtensions {
        /// <summary>
        /// <para>指定された文字列がこのインスタンス内で最後に見つかった場合、</para>
        /// <para>その文字列を削除した新しい文字列を返します</para>
        /// </summary>
        public static string RemoveAtLast(this string self, string value) {
            return self.Remove(self.LastIndexOf(value), value.Length);
        }
    }
}
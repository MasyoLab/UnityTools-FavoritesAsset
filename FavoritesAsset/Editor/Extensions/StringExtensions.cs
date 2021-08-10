
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    static class StringExtensions {
        public static string RemoveAtLast(this string self, string value) {
            return self.Remove(self.LastIndexOf(value), value.Length);
        }
    }
}
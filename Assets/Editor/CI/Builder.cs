using UnityEditor;

namespace CI {
    public static class Builder {
        public static void Build() {
            AssetDatabase.ExportPackage("Assets/FavoritesAsset", "FavoritesAsset.unitypackage", ExportPackageOptions.Recurse);
        }
    }
}
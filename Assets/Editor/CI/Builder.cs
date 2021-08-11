using UnityEditor;

namespace CI {
    public static class Builder {
        public static void Build() {
            AssetDatabase.ExportPackage("Assets/MasyoLab/FavoritesAsset", "FavoritesAsset.unitypackage", ExportPackageOptions.Recurse);
        }
    }
}
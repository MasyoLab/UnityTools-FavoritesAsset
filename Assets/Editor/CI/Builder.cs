using UnityEditor;

namespace CI {
    public static class Builder {
        [MenuItem("Tools/Build Favorites Package")]
        public static void Build() {
            AssetDatabase.ExportPackage("Assets/MasyoLab/FavoritesAsset", "FavoritesAsset.unitypackage", ExportPackageOptions.Recurse);
        }
    }
}

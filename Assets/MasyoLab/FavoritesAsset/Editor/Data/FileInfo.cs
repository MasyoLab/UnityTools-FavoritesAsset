#if UNITY_EDITOR
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {
    public class FileInfo {
        public string FolderDirectory = string.Empty;
        public string Filename = CONST.JSON_DATA_NAME;
        public static FileInfo Empty = new FileInfo(string.Empty, string.Empty);

        public FileInfo(string folderDirectory, string filename) {
            FolderDirectory = folderDirectory;
            Filename = filename;
        }
    }
}
#endif

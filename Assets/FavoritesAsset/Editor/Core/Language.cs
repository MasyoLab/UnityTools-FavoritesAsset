
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    /// <summary>
    /// 言語
    /// </summary>
    [System.Serializable]
    enum LanguageEnum {
        Japanese,
        English,
    }

    /// <summary>
    /// テキスト
    /// </summary>
    enum TextEnum {
        Language,
        DragAndDrop,
        UnlockAll,
        NumFav,
        ChangeDisplay,
        Import,
        Export,
        File,
        Favorites,
        Sort,
        Setting,
        Help,
        ImportAndExportTarget,
        ExportTarget,
        ImportTarget,
    }

    struct LanguageData {
        public static readonly string[] LANGUAGE = {
            "日本語",
            $"{LanguageEnum.English}",
        };

        static readonly string[] TEXT_EN ={
            "Editor Language",
            "Drag & drop to register",
            "Delete all favorites",
            "favourites",
            "Sort Window",
            "Import",
            "Export",
            "File",
            "Favorites",
            "Sort",
            "Setting",
            "Help",
            "Import & Export Target",
            "Export Target",
            "Import Target",
        };

        static readonly string[] TEXT_JP ={
            "エディター言語",
            "ドラッグ＆ドロップで登録",
            "全てのお気に入りを解除",
            "個のお気に入り",
            "表示順の変更",
            "読み込み",
            "名前を付けて保存",
            "ファイル",
            "お気に入り",
            "ソート",
            "設定",
            "ヘルプ",
            "インポート＆エクスポート先",
            "エクスポート先",
            "インポート先",
        };

        public static string GetText(LanguageEnum lang, TextEnum text) {
            switch (lang) {
                case LanguageEnum.English:
                    return TEXT_EN[(int)text];
                case LanguageEnum.Japanese:
                    return TEXT_JP[(int)text];
                default:
                    return TEXT_EN[(int)text];
            }
        }
    }
}
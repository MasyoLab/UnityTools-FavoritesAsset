#if UNITY_EDITOR
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
        English,
        Japanese,
    }

    /// <summary>
    /// テキスト
    /// </summary>
    enum TextEnum {
        Language,
        DragAndDrop,
        UnlockAll,
        NumFav,
        Import,
        Export,
        Menu,
        Favorites,
        Setting,
        Help,
        ImportAndExportTarget,
        ExportTarget,
        ImportTarget,
        Filename,
        SourceCode,
        License,
        LatestRelease,
        Link,
        AddNewFavoriteGroup,
        FavoriteGroup,
        FavoriteGroupIsEmpty,
    }

    struct LanguageData {
        public static readonly string[] LANGUAGE = {
            $"{LanguageEnum.English}",
            "日本語",
        };

        static readonly string[] TEXT_EN ={
            "Editor Language",
            "Drag & drop to register",
            "Delete all favorites",
            "favourites",
            "Import",
            "Export",
            "Menu",
            "Favorites",
            "Setting",
            "Help",
            "Import & Export Target",
            "Export Target",
            "Import Target",
            "Filename",
            "Source Code",
            "License",
            "Latest release",
            "Link",
            "Add New FavoriteGroup...",
            "FavoriteGroup",
            "FavoriteGroup is empty",
        };

        static readonly string[] TEXT_JP ={
            "エディター言語",
            "ドラッグ＆ドロップで登録",
            "全てのお気に入りを解除",
            "個のお気に入り",
            "読み込み",
            "名前を付けて保存",
            "メニュー",
            "お気に入り",
            "設定",
            "ヘルプ",
            "インポート＆エクスポート先",
            "エクスポート先",
            "インポート先",
            "ファイル名",
            "ソースコード",
            "ライセンス",
            "最新のリリース",
            "リンク",
            "お気に入りグループを追加...",
            "お気に入りグループ",
            "お気に入りグループは空です",
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
#endif

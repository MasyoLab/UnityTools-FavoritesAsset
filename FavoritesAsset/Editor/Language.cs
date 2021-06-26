
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
        Japanese
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
    }

    struct LanguageData {
        static public readonly string[] LANGUAGE = {
            $"{LanguageEnum.English}",
            "日本語",
        };

        static readonly string[] TEXT_EN ={
            "Editor Language",
            "Drag and drop to register",
            "Delete all favorites",
            "favourites",
            "Sort Window",
            "Import",
            "Export",
        };
        static readonly string[] TEXT_JP ={
            "エディター言語",
            "ドラッグ＆ドロップで登録",
            "全てのお気に入りを解除",
            "個のお気に入り",
            "表示順の変更",
            "インポート",
            "エクスポート",
        };

        static public string GetText(LanguageEnum lang, TextEnum text) {
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
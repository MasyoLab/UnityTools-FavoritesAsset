using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    struct CONST {
        public const string VERSION = "v1.2.0";
        public const string EDITOR_NAME = "Favorites Asset";
        public const string EDITOR_WINDOW_NAME = "Favorites Asset Window";
        public const string MENU_ITEM = "Tools/" + EDITOR_WINDOW_NAME;
        public const string UNITY_EXT = ".unity";
        public const string JSON_EXT = "favorites";
        public const string SHA256 = "3cf97e6a402faa1f0604b395a0a20228b86431175662ae14ef70beaf1978918b";
        public const string SAVE = "Save";
        public const string LOAD = "Load";
        public const string ASSETS = "Assets";
        public const string LIBRARY = "Library";
        public const string SETTING_DATA = "SettingData";
        public const string FAVORITES_DATA = "FavoritesData";
        public static string FOLDER_NAME => $"{EDITOR_NAME}-{SHA256}";
        public static string JSON_DATA_NAME => $"{Application.productName}-{FAVORITES_DATA}";

        /// <summary>
        /// アイコン：https://github.com/halak/unity-editor-icons
        /// </summary>

        /// <summary>
        /// 目のアイコン
        /// </summary>
        public const string ICON_ANIMATION_VISIBILITY_TOGGLE_ON = "animationvisibilitytoggleon@2x";

        /// <summary>
        /// 閉じるアイコン
        /// </summary>
        public const string ICON_CLOSE = "winbtn_win_close@2x";

        /// <summary>
        /// FolderAdded
        /// </summary>
        public const string ICON_COLLAB_FOLDER_ADDED_D = "d_Collab.FolderAdded";

        /// <summary>
        /// FolderAdded
        /// </summary>
        public const string ICON_COLLAB_FOLDER_ADDED = "Collab.FolderAdded";

        /// <summary>
        /// FileAdded
        /// </summary>
        public const string ICON_COLLAB_FILE_ADDED_D = "d_Collab.FileAdded";

        /// <summary>
        /// FileAdded
        /// </summary>
        public const string ICON_COLLAB_FILE_ADDED = "Collab.FileAdded";

        /// <summary>
        /// Favorite Icon
        /// </summary>
        public const string FAVORITE_ICON = "d_Favorite Icon";

        /// <summary>
        /// Selection List Template Icon
        /// </summary>
        public const string SELECTION_LIST_TEMPLATE_ICON = "d_SelectionListTemplate Icon";

        /// <summary>
        /// Settings Icon
        /// </summary>
        public const string ICON_SETTINGS = "SettingsIcon";

        public const int GUI_LAYOUT_HEIGHT = 22;
    }

    enum WindowEnum {
        FavoritesWindow,
        SortWindow,
        SettingWindow,
        HelpWindow,
        Max,
    }
}

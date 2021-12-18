using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    public class MainWindow : EditorWindow {

        class Pipeline : IPipeline {
            FavoritesManager _favorites = null;
            SettingManager _setting = null;
            GroupManager _group = null;
            DragManager _dragManager = null;

            public FavoritesManager Favorites {
                get {
                    if (_favorites == null) {
                        _favorites = new FavoritesManager(this);
                    }
                    return _favorites;
                }
            }

            public SettingManager Setting {
                get {
                    if (_setting == null) {
                        _setting = new SettingManager(this);
                    }
                    return _setting;
                }
            }

            public GroupManager Group {
                get {
                    if (_group == null) {
                        _group = new GroupManager(this);
                    }
                    return _group;
                }
            }

            public DragManager DragManager {
                get {
                    if (_dragManager == null) {
                        _dragManager = new DragManager(this);
                    }
                    return _dragManager;
                }
            }

            public EditorWindow Root { get; set; } = null;
            public Rect WindowSize { get; set; } = Rect.zero;
        }

        static MainWindow Inst = null;
        List<BaseWindow> _windows = new List<BaseWindow>((int)WindowEnum.Max);
        BaseWindow _guiWindow = null;
        Pipeline _pipeline = new Pipeline();

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        static void Init() {
            var window = GetWindow<MainWindow>(CONST.EDITOR_WINDOW_NAME);
            window.titleContent.image = EditorGUIUtility.IconContent(CONST.FAVORITE_ICON).image;
            Inst = window;
        }

        /// <summary>
        /// エディタ上で選択しているアセットを登録
        /// </summary>
        [MenuItem(CONST.ADD_TO_FAVORITES_ASSET_WINDOW, false, 10001)]
        static void RegisterSelection() {
            if (Selection.activeObject == null)
                return;

            Init();

            var window = Inst.GetWindowClass<FavoritesWindow>();
            foreach (var item in Selection.objects) {
                window.AddAssetToObject(item);
            }
            window.Save();
        }

        /// <summary>
        /// RegisterSelectionのValidateメソッド
        /// </summary>
        [MenuItem(CONST.ADD_TO_FAVORITES_ASSET_WINDOW, true)]
        static bool ValidateRegisterSelection() {
            return Selection.activeObject != null;
        }

        private void OnEnable() {
            _pipeline.Root = this;
            _pipeline.DragManager.OnEnable();
            foreach (var baseWindow in _windows) {
                baseWindow.OnEnable();
            }
        }

        private void OnDestroy() {
            foreach (var baseWindow in _windows) {
                baseWindow.OnDestroy();
            }
        }

        private void OnDisable() {
            foreach (var baseWindow in _windows) {
                baseWindow.OnDisable();
            }
        }

        /// <summary>
        /// GUI 描画
        /// </summary>
        void OnGUI() {
            DrawToolbar();
            UpdateGUIAction();
        }

        void OnFocus() {
            _pipeline.Favorites.CheckFavoritesAsset();
        }

        void UpdateGUIAction() {
            if (_guiWindow == null) {
                GetWindowClass<FavoritesWindow>();
            }

            _pipeline.WindowSize = new Rect(0, EditorStyles.toolbar.fixedHeight, position.width, position.height - EditorStyles.toolbar.fixedHeight);
            _guiWindow.OnGUI();
        }

        _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            if (_guiWindow as _Ty != null) {
                return _guiWindow as _Ty;
            }

            foreach (var item in _windows) {
                _Ty win = item as _Ty;
                if (win == null)
                    continue;
                win.Init(_pipeline);
                _guiWindow?.Close();
                _guiWindow = win;
                return win;
            }

            var newWin = new _Ty();
            newWin.Init(_pipeline);
            _windows.Add(newWin);
            _guiWindow?.Close();
            _guiWindow = newWin;
            return newWin;
        }

        void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                GUIContent content = new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.File));
                if (GUILayout.Button(content, EditorStyles.toolbarDropDown)) {
                    OpenMenu();
                }

                content = new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Favorites));
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    GetWindowClass<FavoritesWindow>();
                }

                var selectIndex = EditorGUILayout.Popup(_pipeline.Group.Index, _pipeline.Group.GroupNames);
                switch (_pipeline.Group.SelectGroupByIndex(selectIndex)) {
                    case GroupSelectEventEnum.Unselect:
                        break;
                    case GroupSelectEventEnum.Select:
                        Reload();
                        break;
                    case GroupSelectEventEnum.Open:
                        GetWindowClass<GroupWindow>();
                        break;
                    default:
                        break;
                }

                GUILayout.FlexibleSpace();
            }
        }

        void OpenMenu() {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Import)), false,
                (call) => {
                    var importJson = SaveLoad.LoadFile(_pipeline.Setting.IOTarget);
                    var importData = FavoritesJsonExportData.FromJson(importJson);
                    _pipeline.Favorites.SetImportData(importData);
                    _pipeline.Group.SetImportData(importData);
                    Reload();
                    GetWindowClass<FavoritesWindow>();
                }, TextEnum.Import);

            menu.AddItem(new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Export)), false,
                (call) => {
                    var exportJson = FavoritesJsonExportData.ToJson(_pipeline.Favorites.AssetInfoList, _pipeline.Group.GroupDB, _pipeline.Favorites.GetFavoriteList());
                    SaveLoad.SaveFile(exportJson, _pipeline.Setting.IOTarget, _pipeline.Setting.IOFileName, result => {
                        _pipeline.Setting.IOTarget = result.FolderDirectory;
                        _pipeline.Setting.IOFileName = result.Filename;
                    });
                }, TextEnum.Export);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.FavoriteGroup)), false,
                (call) => {
                    GetWindowClass<GroupWindow>();
                }, TextEnum.FavoriteGroup);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Setting)), false,
                (call) => {
                    GetWindowClass<SettingWindow>();
                }, TextEnum.Setting);

            menu.AddItem(new GUIContent(LanguageData.GetText(_pipeline.Setting.Language, TextEnum.Help)), false,
                (call) => {
                    GetWindowClass<HelpWindow>();
                }, TextEnum.Help);


            menu.DropDown(new Rect(0, EditorStyles.toolbar.fixedHeight, 0f, 0f));
        }

        void OpenMenuB() {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Favorites"), false,
                (call) => {
                    GetWindowClass<FavoritesWindow>();
                }, "item 1");

            menu.ShowAsContext();

            menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, call => { }, "item 3");

            menu.DropDown(new Rect(0, EditorStyles.toolbar.fixedHeight, 0f, 0f));
        }

        public void Reload() {
            foreach (var item in _windows) {
                item.Reload();
            }
        }
    }
}

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
        List<BaseWindow> _windows = new List<BaseWindow>((int)WindowEnum.Max);
        BaseWindow _guiWindow = null;

        Rect _toolbarSize = Rect.zero;

        /// <summary>
        /// マネージャー
        /// </summary>
        static FavoritesManager _refFavorites = null;
        FavoritesManager _manager {
            get {
                if (_refFavorites == null) {
                    _refFavorites = new FavoritesManager();
                }
                return _refFavorites;
            }
        }

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        static void Init() {
            var window = GetWindow<MainWindow>(CONST.EDITOR_WINDOW_NAME);
            window.titleContent.image = EditorGUIUtility.IconContent(CONST.FAVORITE_ICON).image;
        }

        /// <summary>
        /// GUI 描画
        /// </summary>
        void OnGUI() {
            DrawToolbar();
            UpdateGUIAction();
        }

        void OnFocus() {
            _manager.CheckFavoritesAsset();
        }

        void UpdateGUIAction() {
            if (_guiWindow == null) {
                _guiWindow = GetWindowClass<FavoritesWindow>();
            }

            _guiWindow.OnGUI(new Rect(0, _toolbarSize.y, position.width, position.height - _toolbarSize.height));
        }

        _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            foreach (var item in _windows) {
                _Ty win = item as _Ty;
                if (win == null)
                    continue;
                win.Init(_manager, this);
                return win;
            }

            var newWin = new _Ty();
            newWin.Init(_manager, this);
            _windows.Add(newWin);
            return newWin;
        }

        void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.MinWidth(1))) {
                GUIContent content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.File));
                if (GUILayout.Button(content, EditorStyles.toolbarDropDown)) {
                    OpenMenuA(Vector2.zero);
                }

                content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Favorites));
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiWindow = GetWindowClass<FavoritesWindow>();
                }

                content = new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Sort));
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiWindow = GetWindowClass<SortWindow>();
                }
            }

            _toolbarSize = GUILayoutUtility.GetLastRect();
        }

        void OpenMenuA(Vector2 mousePos) {

            Rect contextRect = new Rect(0, 0, Screen.width, Screen.height);
            if (contextRect.Contains(mousePos)) {
                // Now create the menu, add items and show it
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Import)), false,
                    (call) => {
                        _manager.SetJsonData(SaveLoad.LoadFile(_manager.ImportTarget, (result) => {
                            _manager.ImportTarget = result;
                        }));
                    }, TextEnum.Import);

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Export)), false,
                    (call) => {
                        SaveLoad.SaveFile(_manager.AssetDBJson, _manager.ExportTarget, (result) => {
                            _manager.ExportTarget = result;
                        });
                    }, TextEnum.Export);

                menu.AddSeparator("");

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Setting)), false,
                    (call) => {
                        _guiWindow = GetWindowClass<SettingWindow>();
                    }, TextEnum.Setting);

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Help)), false,
                    (call) => {
                        _guiWindow = GetWindowClass<HelpWindow>();
                    }, TextEnum.Help);

                //menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, call => { }, "item 3");
                menu.ShowAsContext();
            }
        }

        void OpenMenuB(Vector2 mousePos) {

            Rect contextRect = new Rect(0, 0, Screen.width, Screen.height);
            if (contextRect.Contains(mousePos)) {
                // Now create the menu, add items and show it
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Favorites"), false,
                    (call) => {
                        _guiWindow = GetWindowClass<FavoritesWindow>();
                    }, "item 1");

                menu.AddItem(new GUIContent("Sort"), false,
                    (call) => {
                        _guiWindow = GetWindowClass<SortWindow>();
                    }, "item 2");

                menu.ShowAsContext();
            }
        }
    }
}
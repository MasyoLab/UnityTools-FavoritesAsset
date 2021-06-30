using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasyoLab.Editor.FavoritesAsset {

    public class MainWindow : EditorWindow {
        List<BaseWindow> _windows = new List<BaseWindow>((int)WindowEnum.Max);
        UnityEngine.Events.UnityAction _guiAction;

        /// <summary>
        /// マネージャー
        /// </summary>
        static FavoritesManager _ref = null;
        FavoritesManager _manager {
            get {
                if (_ref == null) {
                    _ref = new FavoritesManager();
                }
                return _ref;
            }
        }

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        static void Init() {
            var window = GetWindow<MainWindow>(CONST.EDITOR_NAME);
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
            if (_guiAction == null) {
                _guiAction = GetWindowClass<FavoritesWindow>().OnGUI;
            }
            _guiAction.Invoke();
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
            GUIContent content = null;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.MinWidth(1))) {
                if (GUILayout.Button("File", EditorStyles.toolbarDropDown)) {
                    OpenMenuA(Vector2.zero);
                }

                content = new GUIContent("Favorites");
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiAction = GetWindowClass<FavoritesWindow>().OnGUI;
                }

                content = new GUIContent("Sort");
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiAction = GetWindowClass<SortWindow>().OnGUI;
                }

                content = new GUIContent("Setting");
                content.image = EditorGUIUtility.IconContent(CONST.ICON_SETTINGS).image;
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiAction = GetWindowClass<SettingWindow>().OnGUI;
                }

                content = new GUIContent("License");
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiAction = new BaseWindow().OnGUI;
                }
            }
        }

        void OpenMenuA(Vector2 mousePos) {

            Rect contextRect = new Rect(0, 0, Screen.width, Screen.height);
            if (contextRect.Contains(mousePos)) {
                // Now create the menu, add items and show it
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Import)), false,
                    (call) => {
                        _manager.SetJsonData(SaveLoad.Load());
                    }, "item 1");

                menu.AddItem(new GUIContent(LanguageData.GetText(_manager.Language, TextEnum.Export)), false,
                    (call) => {
                        SaveLoad.Save(_manager.AssetDBJson);
                    }, "item 2");
                //menu.AddSeparator("");
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
                        _guiAction = GetWindowClass<FavoritesWindow>().OnGUI;
                    }, "item 1");

                menu.AddItem(new GUIContent("Sort"), false,
                    (call) => {
                        _guiAction = GetWindowClass<SortWindow>().OnGUI;
                    }, "item 2");

                menu.ShowAsContext();
            }
        }
    }
}
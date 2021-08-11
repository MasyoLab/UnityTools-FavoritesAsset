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

        static PtrLinker<SystemManager> _systemManager = new PtrLinker<SystemManager>();

        SystemManager _manager => _systemManager.Inst;
        FavoritesManager _favorites => _manager.Favorites;
        SettingManager _setting => _manager.Setting;

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
            _favorites.CheckFavoritesAsset();
        }

        void UpdateGUIAction() {
            if (_guiWindow == null) {
                _guiWindow = GetWindowClass<FavoritesWindow>();
            }
            
            _guiWindow.OnGUI(new Rect(0, EditorStyles.toolbar.fixedHeight, position.width, position.height - EditorStyles.toolbar.fixedHeight));
        }

        _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            foreach (var item in _windows) {
                _Ty win = item as _Ty;
                if (win == null)
                    continue;
                win.Init(_systemManager, this);
                return win;
            }

            var newWin = new _Ty();
            newWin.Init(_systemManager, this);
            _windows.Add(newWin);
            return newWin;
        }

        void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                GUIContent content = new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.File));
                if (GUILayout.Button(content, EditorStyles.toolbarDropDown)) {
                    OpenMenuA();
                }

                content = new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Favorites));
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    _guiWindow = GetWindowClass<FavoritesWindow>();
                }

                GUILayout.FlexibleSpace();
            }
        }

        void OpenMenuA() {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Import)), false,
                (call) => {
                    _favorites.SetJsonData(SaveLoad.LoadFile(_setting.ImportTarget, (result) => {
                        _setting.ImportTarget = result;
                    }));
                    Reload();
                }, TextEnum.Import);

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Export)), false,
                (call) => {
                    SaveLoad.SaveFile(_favorites.AssetDBJson, _setting.ExportTarget, (result) => {
                        _setting.ExportTarget = result;
                    });
                }, TextEnum.Export);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Setting)), false,
                (call) => {
                    _guiWindow = GetWindowClass<SettingWindow>();
                }, TextEnum.Setting);

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Help)), false,
                (call) => {
                    _guiWindow = GetWindowClass<HelpWindow>();
                }, TextEnum.Help);


            menu.DropDown(new Rect(0, EditorStyles.toolbar.fixedHeight, 0f, 0f));
        }

        void OpenMenuB() {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Favorites"), false,
                (call) => {
                    _guiWindow = GetWindowClass<FavoritesWindow>();
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
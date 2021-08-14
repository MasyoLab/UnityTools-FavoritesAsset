using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    public class MainWindow : EditorWindow {

        static MainWindow Inst = null;

        List<BaseWindow> _windows = new List<BaseWindow>((int)WindowEnum.Max);
        BaseWindow _guiWindow = null;

        static PtrLinker<SystemManager> _systemManager = new PtrLinker<SystemManager>();

        SystemManager _manager => _systemManager.Inst;
        FavoritesManager _favorites => _manager.Favorites;
        SettingManager _setting => _manager.Setting;
        GroupManager _group => _manager.Group;

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
                GetWindowClass<FavoritesWindow>();
            }

            _guiWindow.OnGUI(new Rect(0, EditorStyles.toolbar.fixedHeight, position.width, position.height - EditorStyles.toolbar.fixedHeight));
        }

        _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            if (_guiWindow as _Ty != null) {
                return _guiWindow as _Ty;
            }

            foreach (var item in _windows) {
                _Ty win = item as _Ty;
                if (win == null)
                    continue;
                win.Init(_systemManager, this);
                _guiWindow?.Close();
                _guiWindow = win;
                return win;
            }

            var newWin = new _Ty();
            newWin.Init(_systemManager, this);
            _windows.Add(newWin);
            _guiWindow?.Close();
            _guiWindow = newWin;
            return newWin;
        }

        void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                GUIContent content = new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.File));
                if (GUILayout.Button(content, EditorStyles.toolbarDropDown)) {
                    OpenMenu();
                }

                content = new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Favorites));
                if (GUILayout.Button(content, EditorStyles.toolbarButton)) {
                    GetWindowClass<FavoritesWindow>();
                }

                var selectIndex = EditorGUILayout.Popup(_group.Index, _group.GroupStr);
                if (_group.SelectGroupByIndex(selectIndex)) {
                    GetWindowClass<GroupWindow>();
                }

                GUILayout.FlexibleSpace();
            }
        }

        void OpenMenu() {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Import)), false,
                (call) => {
                    var importJson = SaveLoad.LoadFile(_setting.ImportTarget, (result) => {
                        _setting.ImportTarget = result;
                    });
                    var importData = FavoritesJsonExportData.FromJson(importJson);
                    _favorites.SetImportData(importData);
                    _group.SetImportData(importData);
                    Reload();
                }, TextEnum.Import);

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Export)), false,
                (call) => {
                    var exportJson = FavoritesJsonExportData.ToJson(_favorites.AssetInfoList, _group.GroupDB, _favorites.GetFavoriteGroups());
                    SaveLoad.SaveFile(exportJson, _setting.ExportTarget, (result) => {
                        _setting.ExportTarget = result;
                    });
                }, TextEnum.Export);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Setting)), false,
                (call) => {
                    GetWindowClass<SettingWindow>();
                }, TextEnum.Setting);

            menu.AddItem(new GUIContent(LanguageData.GetText(_setting.Language, TextEnum.Help)), false,
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

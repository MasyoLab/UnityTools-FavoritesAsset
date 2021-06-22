using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.Security.Cryptography;
using System.Text;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    #region 定数

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
    enum Texts {
        Language,
        DragAndDrop,
        UnlockAll,
        NumFav,
        ChangeDisplay,
    }

    class CONST {
        public const string EDITOR_NAME = "Favorites Asset";
        static public readonly string SORT_WINDOW = $"{EDITOR_NAME}(SortWindow)";
        public const string MENU_ITEM = "Tools/" + EDITOR_NAME;
        public const string UNITY_EXT = ".unity";
        public const string JSON_EXT = "favorites";
        public const string SHA256 = "3cf97e6a402faa1f0604b395a0a20228b86431175662ae14ef70beaf1978918b";

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
        /// レイアウト
        /// </summary>
        public const int GUI_LAYOUT_HEIGHT = 22;

        /// <summary>
        /// 鍵名
        /// </summary>
        static public string DATA_KEY_NAME => $"{Application.productName}-FavoritesData-{SHA256}";
        static public string JSON_DATA_NAME => $"{Application.productName}-FavoritesData";

        static public readonly string[] LANGUAGE = {
            $"{LanguageEnum.English}",
            $"{LanguageEnum.Japanese}",
        };

        static readonly string[] text_en ={
            "Language",
            "Drag and drop to register",
            "Unlock all favorites",
            "Number of Favorites",
            "Change the display order",
        };
        static readonly string[] text_jp ={
            "言語",
            "ドラッグ＆ドロップで登録",
            "全てのお気に入りを解除",
            "お気に入りの数",
            "表示順の変更",
        };

        static public string GetText(LanguageEnum lang, Texts text) {
            switch (lang) {
                case LanguageEnum.English:
                    return text_en[(int)text];
                case LanguageEnum.Japanese:
                    return text_jp[(int)text];
                default:
                    return text_en[(int)text];
            }
        }

        static public string GetSHA256HashString(string value) {
            SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider();
            return string.Join("", provider.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => $"{x:x2}"));
        }
    }
    #endregion

    #region アセットデータ

    /// <summary>
    /// アセットリスト
    /// </summary>
    [System.Serializable]
    class AssetInfo {
        /// <summary>
        /// アセットのGUID
        /// </summary>
        public string Guid = string.Empty;
        /// <summary>
        /// アセットパス
        /// </summary>
        public string Path = string.Empty;
        /// <summary>
        /// アセット名
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// アセットタイプ
        /// </summary>
        public string Type = string.Empty;

        public AssetInfo() { }
        public AssetInfo(string guid, string path, string name, string type) {
            Guid = guid;
            Path = path;
            Name = name;
            Type = type;
        }
    }

    [System.Serializable]
    class AssetInfoList {
        public LanguageEnum Language = LanguageEnum.English;
        public List<AssetInfo> Ref = new List<AssetInfo>();
    }
    #endregion

    static class AssetDrawer {

        /// <summary>
        /// アセットの情報を描画
        /// </summary>
        /// <param name="info"></param>
        /// <param name="onAction"></param>
        static void DrawingSetting(AssetInfo info, UnityEngine.Events.UnityAction<GUIContent, GUIStyle> onAction = null) {
            // 名前を使う
            var content = new GUIContent(info.Name, AssetDatabase.GetCachedIcon(info.Path));

            var style = GUI.skin.button;
            var originalAlignment = style.alignment;
            var originalFontStyle = style.fontStyle;
            var originalTextColor = style.normal.textColor;

            style.alignment = TextAnchor.MiddleLeft;

            onAction?.Invoke(content, style);

            style.alignment = originalAlignment;
            style.fontStyle = originalFontStyle;
            style.normal.textColor = originalTextColor;
        }

        /// <summary>
        /// アセットを開くボタン
        /// </summary>
        /// <param name="info"></param>
        public static void OnAssetButton(Rect rect, AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            DrawingSetting(info, (content, style) => {
                if (GUI.Button(rect, content, style)) {
                    onButtonAction?.Invoke(info);
                }
            });
        }

        /// <summary>
        /// アセットを開くボタン
        /// </summary>
        /// <param name="info"></param>
        public static void OnAssetButton(EditorWindow win, AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            DrawingSetting(info, (content, style) => {
                float width = win.position.width - 100f;
                if (GUILayout.Button(content, style, GUILayout.MaxWidth(width), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                    onButtonAction?.Invoke(info);
                }
            });
        }

        /// <summary>
        /// アセットを開くボタン
        /// </summary>
        /// <param name="info"></param>
        public static void OnAssetButton(AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            DrawingSetting(info, (content, style) => {
                if (GUILayout.Button(content, style, GUILayout.ExpandWidth(true), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                    onButtonAction?.Invoke(info);
                }
            });
        }

        /// <summary>
        /// アセットをPingする
        /// </summary>
        /// <param name="info"></param>
        public static void OnPingObjectButton(AssetInfo info) {
            // アイコンを指定
            var content = EditorGUIUtility.IconContent(CONST.ICON_ANIMATION_VISIBILITY_TOGGLE_ON);
            // ボタン
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                // アセットの情報
                var asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
                EditorGUIUtility.PingObject(asset);
            }
        }

        /// <summary>
        /// お気に入り解除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool OnUnfavoriteButton(AssetInfo info, UnityEngine.Events.UnityAction<AssetInfo> onButtonAction = null) {
            // アイコンを指定
            var content = EditorGUIUtility.IconContent(CONST.ICON_CLOSE);
            // ボタン
            if (GUILayout.Button(content, GUILayout.ExpandWidth(false), GUILayout.Height(CONST.GUI_LAYOUT_HEIGHT))) {
                onButtonAction?.Invoke(info);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Json化
    /// </summary>
    class AssetInfoListToJson {
        public AssetInfoList AssetDB;

        public static string ToJson(AssetInfoList assetInfo) {
            return JsonUtility.ToJson(new AssetInfoListToJson {
                AssetDB = assetInfo,
            });
        }

        public static AssetInfoListToJson FromJson(string jsonData) {
            return JsonUtility.FromJson<AssetInfoListToJson>(jsonData);
        }
    }

    /// <summary>
    /// お気に入りマネージャー
    /// </summary>
    class FavoritesManager {
        /// <summary>
        /// 保存リスト
        /// </summary>
        AssetInfoList _ref = null;
        AssetInfoList _assetInfo {
            get {
                if (_ref == null) {
                    _ref = LoadAssetInfo();
                }
                return _ref;
            }
        }

        /// <summary>
        /// データ
        /// </summary>
        public IReadOnlyList<AssetInfo> Data => _assetInfo.Ref;

        public LanguageEnum Language {
            get => _assetInfo.Language;
            set => _assetInfo.Language = value;
        }

        public string AssetDBJson => AssetInfoListToJson.ToJson(_assetInfo);

        public void Add(AssetInfo info) => _assetInfo.Ref.Add(info);

        public void Add(string guid, string path, string name, string type) {
            _assetInfo.Ref.Add(new AssetInfo(guid, path, name, type));
        }

        public void Remove(AssetInfo info) => _assetInfo.Ref.Remove(info);

        public void RemoveAll() => _assetInfo.Ref.RemoveRange(0, _assetInfo.Ref.Count);

        public bool ExistsGUID(string guid) => _assetInfo.Ref.Exists(x => x.Guid == guid);

        public bool ExistsAssetPath(string path) => _assetInfo.Ref.Exists(x => x.Path == path);

        public void SavePrefs() {
            EditorPrefs.SetString(CONST.DATA_KEY_NAME, JsonUtility.ToJson(_assetInfo));
        }

        AssetInfoList LoadAssetInfo() {
            // データがない
            if (!EditorPrefs.HasKey(CONST.DATA_KEY_NAME))
                return new AssetInfoList();

            string jsonData = EditorPrefs.GetString(CONST.DATA_KEY_NAME);

            // json から読み込む
            var assets = JsonUtility.FromJson<AssetInfoList>(jsonData);
            if (assets == null) {
                return new AssetInfoList();
            }
            return assets;
        }

        /// <summary>
        /// お気に入り登録したアセットを更新
        /// </summary>
        public void CheckFavoritesAsset() {
            foreach (var item in Data) {
                // GUIDでパスを取得
                var newPath = AssetDatabase.GUIDToAssetPath(item.Guid);
                // パスがある
                if (newPath != string.Empty) {
                    item.Path = newPath;
                    // 基本的にここで終わる
                    continue;
                }
            }
            SavePrefs();
        }

        /// <summary>
        /// ソートデータを受け取る
        /// </summary>
        /// <param name="assetInfos"></param>
        public void SortData(in List<AssetInfo> assetInfos) {
            var newData = new List<AssetInfo>(_assetInfo.Ref.Count);

            foreach (var item in assetInfos) {
                var outItem = _assetInfo.Ref.Find(data => data.Guid == item.Guid);
                if (outItem == null)
                    continue;
                newData.Add(outItem);
            }

            _assetInfo.Ref.Clear();
            _assetInfo.Ref.AddRange(newData);
            SavePrefs();
        }

        public void SetJsonData(string jsonData) {
            if (jsonData == string.Empty)
                return;
            _ref = AssetInfoListToJson.FromJson(jsonData).AssetDB;
        }
    }

    /// <summary>
    /// ソート画面
    /// </summary>
    class SortWindow : EditorWindow {
        static SortWindow _inst = null;
        ReorderableList _reorderableList = null;
        List<AssetInfo> _assetInfos = null;
        Vector2 _scrollVec2;

        private void OnGUI() {
            // スクロールビュー
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList?.DoLayoutList();
            GUILayout.EndScrollView();
        }

        public static void Display(EditorWindow win, FavoritesManager manager) {
            _inst?.Close();
            _inst = CreateInstance<SortWindow>();
            _inst.titleContent = new GUIContent(CONST.SORT_WINDOW) {
                image = EditorGUIUtility.IconContent(CONST.SELECTION_LIST_TEMPLATE_ICON).image
            };
            _inst.SetData(win, manager);
            _inst.Show();
        }

        public void SetData(EditorWindow win, FavoritesManager manager) {
            // データを複製
            _assetInfos = manager.Data.ToList();

            // 入れ替え時に呼び出す
            void OnChanged(ReorderableList list) {
                if (_assetInfos == null)
                    return;
                manager?.SortData(_assetInfos);
                // 描画
                win?.Repaint();
            }

            _reorderableList = new ReorderableList(_assetInfos, typeof(GameObject)) {
                drawElementCallback = OnDrawElement,
                onChangedCallback = OnChanged
            };
        }

        void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            AssetDrawer.OnAssetButton(rect, _assetInfos[index]);
        }
    }

    [SerializeField]
    class InOutSys {

        public static void Save(string jsonData) {
            // ファイルパス
            var filePath = EditorUtility.SaveFilePanel("Save", "Assets", CONST.JSON_DATA_NAME, CONST.JSON_EXT);

            // パス無し
            if (string.IsNullOrEmpty(filePath))
                return;

            // 保存処理
            System.IO.File.WriteAllText(filePath, jsonData);
            AssetDatabase.Refresh();
        }

        public static string Load() {
            // ファイルパス
            var filePath = EditorUtility.OpenFilePanel("Load", "Assets", CONST.JSON_EXT);

            // パス無し
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            var reader = new StreamReader(filePath);
            string jsonStr = reader.ReadLine();
            reader.Close();

            return jsonStr;
        }
    }

    /// <summary>
    /// お気に入り登録機能
    /// </summary>
    public class FavoritesAsset : EditorWindow {

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

        static Vector2 _scrollVec2;

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        static void Init() {
            var window = GetWindow<FavoritesAsset>(CONST.EDITOR_NAME);
            window.titleContent.image = EditorGUIUtility.IconContent(CONST.FAVORITE_ICON).image;
        }

        /// <summary>
        /// GUI 描画
        /// </summary>
        void OnGUI() {
            DrawBG();

            // ドラッグアンドドロップ
            DragAndDropGUI();

            // インポート/エクスポート
            ImportExportGUI();

            // プルダウンメニュー
            PulldownMenuGUI();

            // メニュー
            MenuGUI();

            GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));

            // アセット表示
            DrawAssetGUI();

            GUI.Box(GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.Height(20)), $"{CONST.GetText(_manager.Language, Texts.NumFav)} : {_manager.Data.Count}");
        }

        void OnFocus() {
            _manager.CheckFavoritesAsset();
        }

        void DrawBG() {
            { // 左上
                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 右下レイアウト
                style.alignment = TextAnchor.LowerRight;
                GUI.Label(new Rect(position.width * 0f, position.height * 0f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED_D)
                    );
                style.alignment = originalAlignment;
            }
            { // 右上

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.LowerLeft;
                GUI.Label(new Rect(position.width * 0.5f, position.height * 0f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FILE_ADDED)
                    );
                style.alignment = originalAlignment;
            }
            { // 左下

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.UpperRight;
                GUI.Label(new Rect(position.width * 0f, position.height * 0.5f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED)
                    );
                style.alignment = originalAlignment;
            }
            { // 右下

                var style = GUI.skin.label;
                var originalAlignment = style.alignment;
                // 左下レイアウト
                style.alignment = TextAnchor.UpperLeft;
                GUI.Label(new Rect(position.width * 0.5f, position.height * 0.5f, position.width * 0.5f, position.height * 0.5f),
                    EditorGUIUtility.IconContent(CONST.ICON_COLLAB_FOLDER_ADDED_D)
                    );
                style.alignment = originalAlignment;
            }

        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        void DragAndDropGUI() {
            GUI.Box(GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true)), CONST.GetText(_manager.Language, Texts.DragAndDrop));

            if (!GetObjects(out List<string> objs, this))
                return;

            foreach (var item in objs) {
                AddAssetToAssetPath(item);
            }

            _manager.SavePrefs();
        }

        /// <summary>
        /// インポート/エクスポート
        /// </summary>
        void ImportExportGUI() {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Import"), GUILayout.ExpandWidth(true), GUILayout.Height(20))) {
                _manager.SetJsonData(InOutSys.Load());
            }
            if (GUILayout.Button(new GUIContent("Export"), GUILayout.ExpandWidth(true), GUILayout.Height(20))) {
                InOutSys.Save(_manager.AssetDBJson);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// プルダウンメニュー
        /// </summary>
        void PulldownMenuGUI() {
            EditorGUI.BeginChangeCheck();
            _manager.Language = (LanguageEnum)EditorGUILayout.Popup(CONST.GetText(_manager.Language, Texts.Language), (int)_manager.Language, CONST.LANGUAGE);
            EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// メニュー
        /// </summary>
        void MenuGUI() {
            GUILayout.BeginHorizontal();
            {
                var content = new GUIContent(CONST.GetText(_manager.Language, Texts.UnlockAll));
                // お気に入り全解除
                if (GUILayout.Button(content, GUILayout.ExpandWidth(true), GUILayout.Height(40))) {
                    _manager.RemoveAll();
                }
            }
            {
                var content = new GUIContent(CONST.GetText(_manager.Language, Texts.ChangeDisplay));
                if (GUILayout.Button(content, GUILayout.ExpandWidth(true), GUILayout.Height(40))) {
                    SortWindow.Display(this, _manager);
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// アセット表示
        /// </summary>
        void DrawAssetGUI() {
            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            foreach (var info in _manager.Data) {
                // お気に入り登録したアセットを表示
                if (DrawAssetRow(info))
                    break;
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// お気に入りの描画
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool DrawAssetRow(AssetInfo info) {
            GUILayout.BeginHorizontal();

            // Ping を実行
            AssetDrawer.OnPingObjectButton(info);

            // アセットを開くボタン
            AssetDrawer.OnAssetButton(this, info, OpenAsset);

            // お気に入り除ボタン
            bool result = AssetDrawer.OnUnfavoriteButton(info, RemoveAsset);

            GUILayout.EndHorizontal();

            return result;
        }

        /// <summary>
        /// ファイルパスでアセットを登録
        /// </summary>
        void AddAssetToAssetPath(string assetPath) {
            // AssetPathは保存済み
            if (_manager.ExistsAssetPath(assetPath))
                return;

            // GUID を取得
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            // お気に入りに登録
            _manager.Add(guid, assetPath, asset.name, asset.GetType().ToString());
        }

        /// <summary>
        /// GUIDでアセットを登録
        /// </summary>
        void AddAssetToGUID(string assetGuid) {
            // GUIDは保存済み
            if (_manager.ExistsGUID(assetGuid))
                return;

            // GUID からアセットパスを取得
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);

            // アセットの情報
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            // お気に入りに登録
            _manager.Add(assetGuid, path, asset.name, asset.GetType().ToString());
        }

        /// <summary>
        /// お気に入りを解除
        /// </summary>
        /// <param name="info"></param>
        void RemoveAsset(AssetInfo info) {
            _manager.Remove(info);
            _manager.SavePrefs();
        }

        /// <summary>
        /// アセットを開く
        /// </summary>
        /// <param name="info"></param>
        void OpenAsset(AssetInfo info) {
            // シーンアセットを開こうとしている
            if (Path.GetExtension(info.Path).Equals(CONST.UNITY_EXT)) {
                // シーンの保存を促す
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    // 保存をしたので開く
                    EditorSceneManager.OpenScene(info.Path, OpenSceneMode.Single);
                }
                return;
            }

            var asset = AssetDatabase.LoadAssetAtPath<Object>(info.Path);
            // アセットを開く
            AssetDatabase.OpenAsset(asset);
        }

        /// <summary>
        /// ドラッグ&ドロップでオブジェクトを取得
        /// </summary>
        public static bool GetObjects(out List<string> targetList, EditorWindow window, Rect? rect = null) {
            targetList = null;

            // ドラックドロップされたオブジェクトがなければ終わり
            var objectReferences = GetObjects(window, rect);
            if (objectReferences == null)
                return false;

            // ドロップされたオブジェクトに対象の物が無ければ終わり
            targetList = objectReferences.ToList();
            if (targetList.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// ドラッグ&ドロップでオブジェクトを取得
        /// </summary>
        /// <param name="areaTitle"></param>
        /// <param name="widthMin"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static string[] GetObjects(EditorWindow window, Rect? rect = null) {
            var ev = Event.current;

            // エリアが指定されていれば範囲内か確認
            if (rect.HasValue && !rect.Value.Contains(ev.mousePosition))
                return null;

            // ドラッグ＆ドロップで操作が更新された時でも、実行した時でもなければ終わり
            if (ev.type != EventType.DragUpdated && ev.type != EventType.DragPerform)
                return null;

            var paths = DragAndDrop.paths;
            if (mouseOverWindow != window || paths.Length <= 0)
                return null;

            // カーソルに「+」のアイコンを表示
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            // ドラッグ&ドロップで無ければ終わり
            if (ev.type != EventType.DragPerform)
                return null;

            //ドラッグを受け付ける
            DragAndDrop.AcceptDrag();

            DragAndDrop.activeControlID = 0;

            //イベントを使用済みにする
            Event.current.Use();

            return DragAndDrop.paths;
        }
    }
}
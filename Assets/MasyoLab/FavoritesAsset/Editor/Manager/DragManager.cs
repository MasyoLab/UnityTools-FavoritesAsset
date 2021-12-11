using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MasyoLab.Editor.FavoritesAsset {
    class DragManager : BaseManager {

        private bool _isInitialized = false;
        private bool _isDragPerformed = false;
        private bool _isDrag = false;
        private bool _isMove = false;
        private bool _onClick = false;
        AssetData _assetData = null;

        public DragManager(IPipeline pipeline) : base(pipeline) { }
        ~DragManager() {
            OnDestroy();
        }

        public void OnEnable() {
            if (_isInitialized)
                return;
            _isInitialized = true;

            var element = new VisualElement();
            element.style.flexGrow = 1f;

            element.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
            element.RegisterCallback<PointerUpEvent>(OnPointerUpEvent);

            element.RegisterCallback<PointerOutEvent>(OnPointerOutEvent);
            element.RegisterCallback<PointerOverEvent>(OnPointerOverEvent);

            element.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            element.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);

            _pipeline.Root.rootVisualElement.Add(element);
            SceneView.duringSceneGui += sv => OnDragEnd();
            EditorApplication.hierarchyWindowItemOnGUI += (id, rect) => OnDragEnd();
            EditorApplication.projectWindowItemOnGUI += (id, rect) => OnDragEnd();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        void ClearEvent() {
            _onClick = false;
            _isDrag = false;
            _isMove = false;
            _assetData = null;
            DragAndDrop.AcceptDrag();
            DragAndDrop.activeControlID = 0;
            Event.current.Use();
        }

        /// <summary>
        /// クリック時に実行
        /// </summary>
        /// <param name="evt"></param>
        void OnPointerDownEvent(PointerDownEvent evt) {
            Debug.Log(Event.current.type);

            _onClick = true;
            _isDrag = false;
            _isMove = false;
        }

        /// <summary>
        /// エディタ内でクリックが完了した
        /// </summary>
        /// <param name="evt"></param>
        void OnPointerUpEvent(PointerUpEvent evt) {
            ClearEvent();
        }

        void OnDragUpdatedEvent(DragUpdatedEvent evt) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        }

        /// <summary>
        /// ポインターが動いた際に実行
        /// </summary>
        /// <param name="evt"></param>
        void OnPointerMoveEvent(PointerMoveEvent evt) {
            // クリックしていない
            if (!_onClick)
                return;
            _isMove = true;
        }

        /// <summary>
        /// エディタから外れた際に実行
        /// </summary>
        /// <param name="evt"></param>
        void OnPointerOutEvent(PointerOutEvent evt) {
            // クリックしていない
            if (!_onClick)
                return;
            if (Event.current.type != EventType.MouseDrag)
                return;
            if (_assetData == null)
                return;

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.StartDrag("Dragging");
            DragAndDrop.objectReferences = new Object[] { _assetData.GetObject() };
            _isDrag = true;
        }

        /// <summary>
        /// エディタに触れた際に実行
        /// </summary>
        /// <param name="evt"></param>
        void OnPointerOverEvent(PointerOverEvent evt) {
            // クリックしていない,ドラッグ中ではない
            if (!_onClick || !_isDrag)
                return;

            ClearEvent();
        }

        private void OnDragEnd() {

            // 対象にドラッグ
            if (Event.current.type == EventType.DragPerform) {
                _isDragPerformed = true;
            }

            // ドラッグ完了
            if (Event.current.type == EventType.DragExited) {
                if (_isDragPerformed) {
                    _isDragPerformed = false;
                    ClearEvent();
                }
            }
        }

        public void OnMouseDownEvent(Rect rect, int index) {
            if (_isMove)
                return;
            if (_assetData != null)
                return;
            if (Event.current == null)
                return;

            if (Event.current.isMouse && Event.current.button == 0 && rect.Contains(Event.current.mousePosition)) {
                _assetData = _pipeline.Favorites.Data[index];
                DragAndDrop.activeControlID = 0;
                Event.current.Use();
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
        }

        private void OnDestroy() {
            SceneView.duringSceneGui -= sv => OnDragEnd();
            EditorApplication.hierarchyWindowItemOnGUI -= (id, rect) => OnDragEnd();
            EditorApplication.projectWindowItemOnGUI -= (id, rect) => OnDragEnd();
        }
    }
}

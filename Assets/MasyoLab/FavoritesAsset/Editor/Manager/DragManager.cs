using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MasyoLab.Editor.FavoritesAsset {
    class DragManager : BaseManager {

        private bool _isDragPerformed = false;
        private bool _isDragging = false;
        private bool _isInitialized = false;

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

            element.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            element.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
            element.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);

            _pipeline.Root.rootVisualElement.Add(element);
            SceneView.duringSceneGui += sv => OnDragEnd();
            EditorApplication.hierarchyWindowItemOnGUI += (id, rect) => OnDragEnd();
        }

        void OnMouseDownEvent(MouseDownEvent evt) {
            //DragAndDrop.PrepareStartDrag();
            //DragAndDrop.StartDrag("Dragging");
            //Selection.activeGameObject = null;
            Debug.Log("MouseDownEvent!!");
        }

        void OnMouseUpEvent(MouseUpEvent evt) {
            Debug.Log("MouseUpEvent!!");
        }

        void OnDragUpdatedEvent(DragUpdatedEvent evt) {
            //DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            Debug.Log("DragUpdatedEvent!!");
        }

        private void OnDragEnd() {

            if (Event.current.type == EventType.DragPerform) {
                _isDragPerformed = true;
            }

            if (Event.current.type == EventType.DragExited) {
                if (_isDragging && _isDragPerformed) {
                    _isDragging = false;
                    _isDragPerformed = false;

                    var go = Selection.activeGameObject;
                    // Do your **OnDragEnd callback on go** here
                }
            }
        }

        private void OnDestroy() {
            SceneView.duringSceneGui -= sv => OnDragEnd();
            EditorApplication.hierarchyWindowItemOnGUI -= (id, rect) => OnDragEnd();
        }
    }
}

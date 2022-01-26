#if UNITY_EDITOR
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

    class BaseWindow {

        protected IPipeline _pipeline = null;

        public virtual void Init(IPipeline pipeline) {
            _pipeline = pipeline;
        }

        public virtual void OnGUI() { }
        public virtual void Reload() { }
        public virtual void Close() { }

        /// <summary>
        /// この関数は、オブジェクトが有効でアクティブになると呼び出されます
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// EditorWindow が閉じるときに OnDestroy が呼び出される
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// この関数は、オブジェクトが無効か非アクティブになった時点で呼び出されます
        /// </summary>
        public virtual void OnDisable() { }
    }
}
#endif

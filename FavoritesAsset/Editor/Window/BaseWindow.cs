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

        protected FavoritesManager _manager { private set; get; } = null;
        protected EditorWindow _root { private set; get; } = null;
        protected Rect Position => _root.position;

        public virtual void OnGUI() { }
        public virtual void Init(FavoritesManager manager, EditorWindow root) {
            _manager = manager;
            _root = root;
        }
    }
}
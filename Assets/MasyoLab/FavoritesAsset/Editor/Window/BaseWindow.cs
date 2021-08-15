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

        PtrLinker<SystemManager> _systemManager = null;

        protected SystemManager _manager => _systemManager.Inst;
        protected FavoritesManager _favorites => _manager.Favorites;
        protected SettingManager _setting => _manager.Setting;
        protected GroupManager _group => _manager.Group;

        protected EditorWindow _root { private set; get; } = null;

        public virtual void OnGUI(Rect windowSize) { }
        public virtual void Init(PtrLinker<SystemManager> manager, EditorWindow root) {
            _systemManager = manager;
            _root = root;
        }

        public virtual void Reload() { }
        public virtual void Close() { }
    }
}

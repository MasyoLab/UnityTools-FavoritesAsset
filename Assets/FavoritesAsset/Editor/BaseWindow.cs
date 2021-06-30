using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasyoLab.Editor.FavoritesAsset {

    abstract class BaseWindow {
        protected FavoritesManager _manager = null;

        public abstract void OnGUI();
        public virtual void SetFavoritesManager(FavoritesManager manager) {
            _manager = manager;
        }
    }
}
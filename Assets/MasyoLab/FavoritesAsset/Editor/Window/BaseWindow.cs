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

        public virtual void OnGUI() { }
        public virtual void Init(IPipeline pipeline) {
            _pipeline = pipeline;
        }
        public virtual void Reload() { }
        public virtual void Close() { }
    }
}

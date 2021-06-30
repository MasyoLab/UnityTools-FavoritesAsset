using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasyoLab.Editor.FavoritesAsset {

    public class MainWindow : EditorWindow {
        List<BaseWindow> _windows = new List<BaseWindow>();

        _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            foreach (var item in _windows) {
                _Ty win = item as _Ty;
                if (win == null)
                    continue;
                return win;
            }

            var newWin = new _Ty();
            _windows.Add(newWin);
            return newWin;
        }
    }
}
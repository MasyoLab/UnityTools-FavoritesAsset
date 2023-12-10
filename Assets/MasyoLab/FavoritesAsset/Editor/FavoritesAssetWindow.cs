using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MasyoLab.Editor
{
    public static class FavoritesAssetWindow
    {
        //
        // Summary:
        //      Extends the icon retrieval process for displaying the icon of favorited assets.
        //      The specified Texture in the return value takes precedence.
        //      If null is returned, the existing icon loading process will be executed.
        //
        // Parameter:
        //     path:
        //       asset path.
        //
        //   Return:
        //     The Texture used for displaying the icon.
        public delegate Texture GetTextureFunction(string path);
        public static GetTextureFunction GetFavoritesAssetIcon;
    }
}

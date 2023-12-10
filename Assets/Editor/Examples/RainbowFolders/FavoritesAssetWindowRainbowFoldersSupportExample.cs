/*
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MasyoLab.Editor;
using Borodar.RainbowFolders.Editor.Settings;

/// <summary>
/// Compatibility with RainbowFolders in the Favorites Assets window.
/// </summary>
[InitializeOnLoad]
public class FavoritesAssetWindowRainbowFoldersSupportExample
{
    static FavoritesAssetWindowRainbowFoldersSupportExample()
    {
        FavoritesAssetWindow.GetFavoritesAssetIcon += GetAssetIcon;
    }

    private static Texture GetAssetIcon(string folderPath)
    {
        var setting = RainbowFoldersSettings.Instance;
        if (setting == null)
        {
            return null;
        }
        var texture = setting.GetFolderIcon(folderPath, true);
        if (texture == null)
        {
            return null;
        }
        return texture;
    }
}
*/

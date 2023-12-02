#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset
{
    interface IPipeline
    {
        FavoritesManager Favorites { get; }
        SettingManager Setting { get; }
        GroupManager Group { get; }
        EditorWindow Root { get; }
        Rect WindowSize { get; }
    }
}
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {
    class TextureManager : BaseManager {

        Dictionary<string, IReadOnlyList<Texture>> _textureDict = new Dictionary<string, IReadOnlyList<Texture>>();

        public TextureManager(IPipeline pipeline) : base(pipeline) { }

        public Texture LoadAssetIcon(AssetData assetData) {

            switch (assetData.Type) {

                case "UnityEngine.Sprite": {
                    return LoadTexture(assetData);
                }

                default:
                    break;
            }
            return AssetDatabase.GetCachedIcon(assetData.Path);
        }

        static Texture2D TrimTexture(string name, Texture2D source, Rect uvRect, Vector2Int destSize) {
            var currentRT = RenderTexture.active;

            var tempRT = RenderTexture.GetTemporary(destSize.x, destSize.y, 24);

            // RenderTexure.activeを差し変えておく
            RenderTexture.active = tempRT;

            // 切り取り領域を指定しつつsourceをRenderTextureにコピーする
            Graphics.Blit(source, tempRT, uvRect.size, uvRect.min);

            // RenderTexture.activeの情報をテクスチャに書き込む
            var texture = new Texture2D(destSize.x, destSize.y, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, destSize.x, destSize.y), 0, 0);
            texture.name = name;
            texture.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(tempRT);
            return texture;
        }

        Texture LoadTexture(AssetData assetData) {

            if (_textureDict.ContainsKey(assetData.Guid)) {
                return _textureDict[assetData.Guid].FirstOrDefault(v => v.name == assetData.Name);
            }

            var objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetData.Path);
            if (objects.Length == 0) {
                return null;
            }

            if (objects.Length == 1) {
                return AssetDatabase.GetCachedIcon(assetData.Path);
            }

            var sprites = new List<Texture>(objects.Length);
            foreach (var item in objects) {
                var sprite = item as Sprite;
                if (sprite == null)
                    continue;


                // 書き換え用テクスチャの生成
                //change_texture.filterMode = sprite.texture.filterMode;

                var rect = new Rect();
                rect.min = new Vector2(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height);
                rect.max = new Vector2((sprite.rect.x + sprite.rect.width) / sprite.texture.width, (sprite.rect.y + sprite.rect.height) / sprite.texture.height);

                var texture = TrimTexture(item.name, sprite.texture, rect, new Vector2Int((int)(sprite.rect.width + 0.5f), (int)(sprite.rect.height + 0.5f)));
                sprites.Add(texture);
            }
            if (sprites.Count == 0) {
                return null;
            }

            _textureDict.Add(assetData.Guid, sprites);
            return _textureDict[assetData.Guid].FirstOrDefault(v => v.name == assetData.Name);
        }
    }
}




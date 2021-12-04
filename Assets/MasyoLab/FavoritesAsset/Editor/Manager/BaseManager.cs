
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {
    class BaseManager {
        protected IPipeline _pipeline = null;

        public BaseManager(IPipeline pipeline) {
            _pipeline = pipeline;
        }
    }
}
#if UNITY_EDITOR
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class BaseManager {
        protected IPipeline m_pipeline = null;

        public BaseManager(IPipeline pipeline) {
            m_pipeline = pipeline;
        }
    }
}
#endif

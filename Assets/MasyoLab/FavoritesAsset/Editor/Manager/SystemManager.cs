
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class SystemManager {
        PtrLinker<FavoritesManager> _favorites = new PtrLinker<FavoritesManager>();
        public FavoritesManager Favorites => _favorites.Inst;

        PtrLinker<SettingManager> _setting = new PtrLinker<SettingManager>();
        public SettingManager Setting => _setting.Inst;
    }
}
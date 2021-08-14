
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class SystemManager {
        PtrLinker<FavoritesManager> _favorites = null;
        public FavoritesManager Favorites => _favorites.Inst;

        PtrLinker<SettingManager> _setting = null;
        public SettingManager Setting => _setting.Inst;

        PtrLinker<GroupManager> _group = null;
        public GroupManager Group => _group.Inst;

        public SystemManager() {
            _favorites = new PtrLinker<FavoritesManager>();
            _setting = new PtrLinker<SettingManager>();
            _group = new PtrLinker<GroupManager>();
            
            _favorites.Inst.SetGroupManager(_group);
        }
    }
}

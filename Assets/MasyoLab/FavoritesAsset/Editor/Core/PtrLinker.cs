
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class PtrLinker<_Ty> where _Ty : new() {

        System.Func<_Ty> _initFunc;

        public PtrLinker() {
            _initFunc = () => new _Ty();
        }
        public PtrLinker(System.Func<_Ty> initFunc) {
            _initFunc = initFunc;
        }

        _Ty _ptr = default;
        public _Ty Inst {
            get {
                if (_ptr == null) {
                    _ptr = _initFunc();
                }
                return _ptr;
            }
        }

        public void SetInst(_Ty inst) => _ptr = inst;
    }
}

#if UNITY_EDITOR
//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-FavoritesAsset
//
//=========================================================

namespace MasyoLab.Editor.FavoritesAsset {

    class PtrLinker<_Ty> where _Ty : new() {

        private System.Func<_Ty> m_initFunc = null;
        private _Ty m_ptr = default;

        public PtrLinker() {
            m_initFunc = () => new _Ty();
        }

        public PtrLinker(System.Func<_Ty> initFunc) {
            m_initFunc = initFunc;
        }

        public _Ty Inst {
            get {
                if (m_ptr == null) {
                    m_ptr = m_initFunc();
                }
                return m_ptr;
            }
        }

        public void SetInst(_Ty inst) => m_ptr = inst;
    }
}
#endif

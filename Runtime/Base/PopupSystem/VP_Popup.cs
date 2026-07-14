namespace HannibalUI.Runtime.Base
{
    using UnityEngine;
    using UnityEngine.Pool;

    public class VP_Popup : MonoBehaviour
    {
        private IObjectPool<VP_Popup> _pool;

        public void SetPool(IObjectPool<VP_Popup> pool)
        {
            _pool = pool;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        /// <summary>Return this popup to its pool (or just hide it if it isn't pooled).</summary>
        public void Close()
        {
            if (_pool != null)
            {
                _pool.Release(this);
                return;
            }

            Deactivate();
        }
    }
}

namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;

    /// <summary>
    /// Shows and hides <see cref="VP_Popup"/> instances on a layer above the screens, keeping one
    /// <see cref="ObjectPool{T}"/> per popup prefab. Popups overlay the active screen without
    /// deactivating it; the layer's own Canvas sorting order is what puts them on top.
    /// </summary>
    public class VP_PopupManager : IDisposable
    {
        private readonly Transform _layer;
        private readonly int _defaultCapacity;
        private readonly int _maxSize;
        private readonly Dictionary<VP_Popup, IObjectPool<VP_Popup>> _pools =
            new Dictionary<VP_Popup, IObjectPool<VP_Popup>>();

        public VP_PopupManager(Transform layer, int defaultCapacity = 4, int maxSize = 20)
        {
            _layer = layer;
            _defaultCapacity = defaultCapacity;
            _maxSize = maxSize;
        }

        public VP_Popup Show(VP_Popup prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("VP_PopupManager: cannot show a null popup prefab.");
                return null;
            }

            return GetPool(prefab).Get();
        }

        public void Hide(VP_Popup popup)
        {
            if (popup != null)
            {
                popup.Close();
            }
        }

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }

            _pools.Clear();
        }

        private IObjectPool<VP_Popup> GetPool(VP_Popup prefab)
        {
            if (_pools.TryGetValue(prefab, out var existing))
            {
                return existing;
            }

            IObjectPool<VP_Popup> pool = null;
            pool = new ObjectPool<VP_Popup>(
                createFunc: () => CreatePopup(prefab, pool),
                actionOnGet: popup => popup.Activate(),
                actionOnRelease: popup => popup.Deactivate(),
                actionOnDestroy: popup => UnityEngine.Object.Destroy(popup.gameObject),
                collectionCheck: true,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize);

            _pools.Add(prefab, pool);
            return pool;
        }

        private VP_Popup CreatePopup(VP_Popup prefab, IObjectPool<VP_Popup> pool)
        {
            var popup = UnityEngine.Object.Instantiate(prefab, _layer);
            popup.SetPool(pool);
            popup.Deactivate();
            return popup;
        }
    }
}

namespace HannibalUI.Runtime.Helpers.Memory 
{
    using System;
    using UnityEngine;

    public class ObjectPool<TObject> where TObject : IPoolable
    {
        private TObject[] _pool;
        private int _currentIndex;
        private GameObject _prefab;

        public ObjectPool(int population, GameObject prefab)
        {
            _prefab = prefab;
            InitializePool(population);
        }

        private ObjectPool() { }

        private void InitializePool(int population)
        {
            _pool = new TObject[population];
            _currentIndex = 0;

            for (int objectIndex = 0; objectIndex < population; objectIndex++)
            {
                _pool[objectIndex] = GenerateObject();
            }
        }

        private TObject GenerateObject()
        {
            var poolable = GameObject.Instantiate(_prefab).GetComponent<TObject>();
            poolable.Initialize();
            poolable.Deactivate();
            return poolable;
        }

        public TObject Get()
        {
            if (_currentIndex == _pool.Length)
            {
                Array.Resize(ref _pool, _pool.Length * 2);
            }

            TObject tObject = _pool[_currentIndex];
            tObject.Activate();
            _currentIndex++;

            return tObject;
        }

        public void Release(TObject obj)
        {
            obj.Deactivate();
            _currentIndex--;
            _pool[_currentIndex] = obj;
        }
    }
}


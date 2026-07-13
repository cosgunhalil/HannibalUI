namespace HannibalUI.Tests.Runtime 
{
    using System.Collections;
    using System.Collections.Generic;
    using HannibalUI.Runtime.Helpers.Memory;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class ObjectPoolTests
    {
        private ObjectPool<PoolableTestObject> objectPool;
        private GameObject objectPrefab;

        [SetUp]
        public void Setup()
        {
            objectPrefab = new GameObject();
            objectPrefab.AddComponent<PoolableTestObject>();
            objectPool = new ObjectPool<PoolableTestObject>(10, objectPrefab);
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(objectPrefab);
            objectPool = null;
        }

        [Test]
        public void GetObjectFromPool()
        {
            PoolableTestObject obj = objectPool.Get();
            Assert.IsTrue(obj.IsActive());
        }

        [Test]
        public void ReleaseObjectToPool()
        {
            PoolableTestObject obj = objectPool.Get();
            objectPool.Release(obj);
            Assert.IsFalse(obj.IsActive());
        }

        [Test]
        public void ExtendPool()
        {
            for (int i = 0; i < 20; i++)
            {
                PoolableTestObject obj = objectPool.Get();
                Assert.IsTrue(obj.IsActive());
                objectPool.Release(obj);
            }
        }

        [Test]
        public void ReleaseMoreThanGetDoesNotCorruptPool()
        {
            PoolableTestObject obj = objectPool.Get();
            objectPool.Release(obj);

            LogAssert.Expect(LogType.Warning, "Release was called more times than Get; ignoring to avoid corrupting the pool.");
            Assert.DoesNotThrow(() => objectPool.Release(obj));

            PoolableTestObject next = objectPool.Get();
            Assert.IsTrue(next.IsActive());
        }
    }

    public class PoolableTestObject : MonoBehaviour, IPoolable
    {
        private bool _isActive;

        public void Activate()
        {
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }

        public void Initialize()
        {

        }

        public bool IsActive()
        {
            return _isActive;
        }
    }

}

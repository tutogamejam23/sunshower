using UnityEngine;
using UnityEngine.Pool;
using System;

namespace Sunshower
{
    public class BehaviourPool<T> : IObjectPool<T>, IDisposable where T : MonoBehaviour
    {
        private readonly IObjectPool<T> _pool;
        private readonly T _original;

        public int CountInactive => _pool.CountInactive;

        public BehaviourPool(T original)
        {
            Debug.Assert(original);
            _pool = new ObjectPool<T>(OnCreate, OnGet, OnRelease, OnDestroy);
            _original = original;
        }

        private T OnCreate()
        {
            var clone = UnityEngine.Object.Instantiate(_original);
            return clone;
        }

        private void OnGet(T behaviour)
        {
            behaviour.gameObject.SetActive(true);
        }

        private void OnRelease(T behaviour)
        {
            behaviour.gameObject.SetActive(false);
        }

        private void OnDestroy(T behaviour)
        {
            UnityEngine.Object.Destroy(behaviour.gameObject);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public T Get()
        {
            return _pool.Get();
        }

        public PooledObject<T> Get(out T v)
        {
            return _pool.Get(out v);
        }

        public void Release(T element)
        {
            _pool.Release(element);
        }

        public void Dispose()
        {
            _pool.Clear();
        }
    }
}

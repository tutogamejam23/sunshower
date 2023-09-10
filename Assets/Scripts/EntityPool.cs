using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    public interface IEntityPoolItem<T> : IPoolItem<T, GameEntityData> where T : MonoBehaviour
    {
    }

    public class EntityPool<T> : IObjectPool<T> where T : MonoBehaviour, IEntityPoolItem<T>
    {
        private readonly IObjectPool<T> _pool;
        private readonly GameEntityData _data;

        public GameEntityData Data => _data;
        public int CountInactive => _pool.CountInactive;
        public IReadOnlyCollection<T> InactiveObjects => _inactiveObjects;

        private readonly LinkedList<T> _inactiveObjects;

        public EntityPool(GameEntityData data)
        {
            _pool = new ObjectPool<T>(CreateInstance, OnGetFromPool, OnRealseToPool, OnDestroyPooledObject);
            _data = data;
            _inactiveObjects = new LinkedList<T>();
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

        private T CreateInstance()
        {
            var prefab = Resources.Load<GameObject>($"Prefabs/{_data.PrefabPath}");
            var go = Object.Instantiate(prefab);
            if (!go.TryGetComponent(out T intance))
            {
                Debug.LogError($"{_data.PrefabPath}에 {typeof(T)} 컴포넌트가 존재하지 않습니다!");
                return null;
            }
            intance.Initialize(_pool, _data);
            return intance;
        }

        private void OnRealseToPool(T pooledObject)
        {
            _inactiveObjects.AddLast(pooledObject);
            pooledObject.gameObject.SetActive(false);
        }

        private void OnGetFromPool(T pooledObject)
        {
            _inactiveObjects.Remove(pooledObject);
            pooledObject.gameObject.SetActive(true);
        }

        private void OnDestroyPooledObject(T pooledObject)
        {
            Object.Destroy(pooledObject.gameObject);
        }
    }
}

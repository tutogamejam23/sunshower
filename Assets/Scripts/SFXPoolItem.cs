using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXPoolItem : MonoBehaviour
    {
        public SFXPool Parent { get; private set; }
        public AudioSource Source { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Start()
        {
            Source = gameObject.GetComponent<AudioSource>();
            Debug.Assert(Source);
        }

        private void Update()
        {
            if (IsInitialized && !Source.isPlaying)
            {
                IsInitialized = false;
                transform.SetParent(null);
                Parent.Release(this);
            }
        }

        public void Register(SFXPool parent)
        {
            Parent = parent;
        }

        public void PlayAtPosition(AudioClip clip, Vector3 position)
        {
            var source = gameObject.GetComponent<AudioSource>();
            source.transform.position = position;
            source.clip = clip;
            source.loop = false;
            source.Play();

            IsInitialized = true;
        }

        public void Play(AudioClip clip)
        {
            transform.SetParent(Camera.main.transform);
            Source.clip = clip;
            Source.loop = false;
            Source.Play();

            IsInitialized = true;
        }
    }

    public class SFXPool : IObjectPool<SFXPoolItem>
    {
        private readonly IObjectPool<SFXPoolItem> _pool;
        private readonly SFXPoolItem _original;

        public int CountInactive => _pool.CountInactive;

        public SFXPool(SFXPoolItem original)
        {
            _pool = new ObjectPool<SFXPoolItem>(CreateInstance, OnGetFromPool, OnRealseToPool, OnDestroyPooledObject);
            _original = original;
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public SFXPoolItem Get()
        {
            return _pool.Get();
        }

        public PooledObject<SFXPoolItem> Get(out SFXPoolItem v)
        {
            return _pool.Get(out v);
        }

        public void Release(SFXPoolItem element)
        {
            _pool.Release(element);
        }

        private SFXPoolItem CreateInstance()
        {
            var instance = UnityEngine.Object.Instantiate(_original);
            instance.Register(this);
            return instance;
        }

        private void OnGetFromPool(SFXPoolItem item)
        {
            item.gameObject.SetActive(true);
        }

        private void OnRealseToPool(SFXPoolItem item)
        {
            item.gameObject.SetActive(false);
        }

        private void OnDestroyPooledObject(SFXPoolItem item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}

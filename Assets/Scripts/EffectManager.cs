using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private EffectTable _effectTable;

        private readonly Dictionary<string, BehaviourPool<Effect>> _pools = new();

        private void Awake()
        {
            Debug.Assert(_effectTable);
            _effectTable.CreateTable();
        }

        public Effect Play(string name, Vector3 position)
        {
            if (!_pools.TryGetValue(name, out var pool))
            {
                pool = new BehaviourPool<Effect>(_effectTable.Effects[name]);
                _pools.Add(name, pool);
            }

            var effect = pool.Get();
            effect.Pool = pool;
            effect.transform.position = position;
            effect.Play();

            return effect;
        }
    }
}

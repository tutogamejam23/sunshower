using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    [CreateAssetMenu(fileName = "EffectTable", menuName = "ScriptableObjects/EffectTable")]
    public class EffectTable : ScriptableObject
    {
        [SerializeField] private List<Effect> _effects = new();

        public Dictionary<string, Effect> Effects { get; private set; }

        public void CreateTable()
        {
            Effects = new Dictionary<string, Effect>();
            foreach (var effect in _effects)
            {
                if (effect == null)
                {
                    continue;
                }
                Effects.TryAdd(effect.name, effect);
            }
        }
    }
}

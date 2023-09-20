using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleEffect : Effect
    {
        private ParticleSystem _particleSystem;
        private bool _isPlayed;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (_isPlayed && !_particleSystem.IsAlive())
            {
                _isPlayed = false;
                Pool.Release(this);
            }
        }

        public override void Play()
        {
            _particleSystem.Play();
            _isPlayed = true;
        }
    }
}

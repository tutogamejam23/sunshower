using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Sunshower
{
    public class AnimationEffect : Effect
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _direction;
        [SerializeField] private float _range;
        [SerializeField] private float _duration;
        [SerializeField] private Ease Ease;

        public override void Play()
        {
            transform.position += _offset;
            transform.DOMove(transform.position + _direction * _range, _duration)
                .SetEase(Ease)
                .OnComplete(() => Pool.Release(this));
        }
    }
}

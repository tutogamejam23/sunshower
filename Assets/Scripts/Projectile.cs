using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    public enum ShotType
    {
        Straight,
        Curve,
        Homing
    }

    public class Projectile : MonoBehaviour
    {
        [SerializeField] private string _shotSFX = string.Empty;
        [SerializeField] private string _hitEffect = string.Empty;
        [SerializeField] private string _hitSFX = string.Empty;

        public IObjectPool<Projectile> Pool { get; set; }
        public IGameEntity Owner { get; set; }
        public ShotType Shot { get; set; }
        public Vector3 Direction { get; set; }
        public float Speed { get; set; }
        public EntitySideType Target { get; set; }
        public int HitCount { get; set; }

        private int _hitCount;

        public SkillCommand.HitSideEffect OnHit { get; set; }

        private void Update()
        {
            ProjectileTypeShot(Shot);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out IGameEntity entity))
            {
                return;
            }

            if (entity == Owner || entity.EntitySide != Target || entity.HP == 0)
            {
                return;
            }

            if (_hitCount >= HitCount)
            {
                return;
            }
            Hit(entity);
        }

        public void OnShot()
        {
            _hitCount = 0;
            if (_shotSFX != string.Empty)
            {
                SoundManager.instance.PlaySFXAtPosition(_shotSFX, transform.position);
            }
        }

        private void Hit(IGameEntity target)
        {
            if (OnHit.Damage > 0)
            {
                target.HP -= OnHit.Damage;
            }
            if (OnHit.Buff != null)
            {
                switch (target)
                {
                    case Player player:
                        player.SkillManager.AddBuff(OnHit.Buff);
                        break;

                    case Mob mob:
                        mob.SkillManager.AddBuff(OnHit.Buff);
                        break;
                    default:
                        break;
                }
            }

            if (_hitEffect != string.Empty)
            {
                Stage.Instance.EffectManager.Play(_hitEffect, transform.position);
            }
            if (_hitSFX != string.Empty)
            {
                SoundManager.instance.PlaySFXAtPosition(_hitSFX, transform.position);
            }

            if (++_hitCount >= HitCount)
            {
                Pool.Release(this);
            }

        }

        public void ProjectileTypeShot(ShotType Type)
        {
            switch (Type)
            {
                case ShotType.Straight:
                    transform.position += Speed * Time.deltaTime * Direction;
                    break;

                case ShotType.Curve:
                    //#TODD : 삼각함수 이용
                    break;

                case ShotType.Homing:
                    //#TODD : 두트원 이용
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            // transform.rotation = Quaternion.Euler(Direction);
        }
    }
}

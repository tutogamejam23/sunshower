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
        public IGameEntity Owner { get; set; }
        public ShotType Shot { get; set; }
        public Vector3 Direction { get; set; }

        public IObjectPool<Projectile> Pool { get; set; }

        public float Speed { get; set; }
        public SkillCommand.HitSideEffect OnHit { get; set; }

        private void Update()
        {
            ProjectileTypeShot(Shot);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out IGameEntity entity) || entity != Owner)
            {
                return;
            }
            Hit(entity);
        }

        private void Hit(IGameEntity target)
        {

        }

        public void ProjectileTypeShot(ShotType Type)
        {
            switch (Type)
            {
                case ShotType.Straight:
                    transform.Translate(Speed * Time.deltaTime * Direction);
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
        }

        public void SetPool(IObjectPool<Projectile> Pool) => this.Pool = Pool;

        public void ReturnPool(IObjectPool<Projectile> Pool) => Pool.Release(this);
    }
}

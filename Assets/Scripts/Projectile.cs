using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public float Speed { get; set; }
        public SkillCommand.HitSideEffect OnHit { get; set; }

        private void Update()
        {
            transform.Translate(Speed * Time.deltaTime * Direction);
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
    }
}

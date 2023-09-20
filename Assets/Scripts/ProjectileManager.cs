using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sunshower.SkillCommand;

namespace Sunshower
{

    public class ProjectileManager : MonoBehaviour
    {
        [SerializeField] private ProjectileTable _projectileTable;

        private readonly Dictionary<string, BehaviourPool<Projectile>> _pools = new();

        private void Awake()
        {
            Debug.Assert(_projectileTable);
            _projectileTable.CreateTable();
        }

        public Projectile Shot(string name, Vector3 position, Vector3 direction, float speed, EntitySideType target, int hitCount, HitSideEffect onHit)
        {
            if (!_pools.TryGetValue(name, out var pool))
            {
                pool = new BehaviourPool<Projectile>(_projectileTable.Projectiles[name]);
                _pools.Add(name, pool);
            }

            var projectile = pool.Get();
            projectile.Pool = pool;
            projectile.transform.position = position;
            projectile.Direction = direction;
            projectile.Target = target;
            projectile.Speed = speed;
            projectile.OnHit = onHit;
            projectile.HitCount = hitCount;

            projectile.OnShot();

            return projectile;
        }
    }
}

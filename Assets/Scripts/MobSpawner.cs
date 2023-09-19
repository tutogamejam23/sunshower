using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Sunshower
{
    public class MobSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _friendlySpawnPoint;
        [SerializeField] private Transform _enemySpawnPoint;

        public IReadOnlyCollection<Mob> ActiveFriendlyMobs => _activeFriendlyMobs;
        public IReadOnlyCollection<Mob> ActiveEnemyMobs => _activeEnemyMobs;

        private readonly Dictionary<int, EntityPool<Mob>> _mobPool = new();
        private readonly LinkedList<Mob> _activeFriendlyMobs = new();
        private readonly LinkedList<Mob> _activeEnemyMobs = new();
        private Unity.Mathematics.Random _random;

        private void Awake()
        {
            _random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        }

        private void Start()
        {
            Debug.Assert(_friendlySpawnPoint, "Friendly Spawn Point가 연결되어 있지 않습니다!");
            Debug.Assert(_enemySpawnPoint, "Enemy Spawn Point가 연결되어 있지 않습니다!");

            foreach (var (id, mobData) in Stage.Instance.DataTable.MobTable)
            {
                _mobPool.Add(id, new EntityPool<Mob>(mobData, onRelease: OnReleaseFromPool));
            }
        }

        public void Spawn(int mobID, EntitySideType side)
        {
            var mob = _mobPool[mobID].Get();
            mob.transform.position = side switch
            {
                EntitySideType.Friendly => _friendlySpawnPoint.position,
                EntitySideType.Enemy => _enemySpawnPoint.position,
                _ => throw new System.NotImplementedException()
            };
            var y = mob.transform.position.y;
            y = _random.NextFloat(y - 0.3f, y + 0.3f);
            mob.transform.position = new Vector3(mob.transform.position.x, y, mob.transform.position.z);

            mob.HP = mob.Data.HP;
            mob.Direction = transform.localScale = side == EntitySideType.Friendly ? Vector3.right : Vector3.left;
            mob.EntitySide = side;
            switch (side)
            {
                case EntitySideType.Friendly:
                    _activeFriendlyMobs.AddLast(mob);
                    break;
                case EntitySideType.Enemy:
                    _activeEnemyMobs.AddLast(mob);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            mob.OnActive();
        }

        public void OnReleaseFromPool(Mob mob)
        {
            switch (mob.EntitySide)
            {
                case EntitySideType.Friendly:
                    _activeFriendlyMobs.Remove(mob);
                    break;
                case EntitySideType.Enemy:
                    _activeEnemyMobs.Remove(mob);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}

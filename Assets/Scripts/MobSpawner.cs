using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Sunshower
{
    public class MobSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _friendlySpawnPoint;
        [SerializeField] private Transform _enemySpawnPoint;

        public IEnumerable<Mob> ActiveMobs => _mobPool.Values.SelectMany(pool => pool.InactiveObjects);

        /// <summary>
        /// Linq를 이용하여 모든 활성화 몹 갯수를 반환합니다. 성능 이슈가 있을 수 있습니다.
        /// </summary>
        public int MobCount => _mobPool.Values.Sum(pool => pool.CountInactive);

        /// <summary>
        /// Linq를 이용하여 모든 활성화 아군 몹 갯수를 반환합니다. 성능 이슈가 있을 수 있습니다.
        /// </summary>
        public int ActiveFriendlyMobCount => _mobPool.Values.Sum(pool => pool.InactiveObjects.Count(mob => mob.MobType == MobType.Friendly));

        /// <summary>
        /// Linq를 이용하여 모든 활성화 적 몹 갯수를 반환합니다. 성능 이슈가 있을 수 있습니다.
        /// </summary>
        public int ActiveEnemyMobCount => _mobPool.Values.Sum(pool => pool.InactiveObjects.Count(mob => mob.MobType == MobType.Enemy));

        private readonly Dictionary<int, EntityPool<Mob>> _mobPool = new();

        private void Start()
        {
            Debug.Assert(_friendlySpawnPoint, "Friendly Spawn Point가 연결되어 있지 않습니다!");
            Debug.Assert(_enemySpawnPoint, "Enemy Spawn Point가 연결되어 있지 않습니다!");

            foreach (var (id, mobData) in Stage.Instance.DataTable.MobTable)
            {
                _mobPool.Add(id, new EntityPool<Mob>(mobData));
            }
        }

        public void Spawn(int mobID, MobType mobType)
        {
            var mob = _mobPool[mobID].Get();
            mob.MobType = mobType;
            mob.transform.position = mobType switch
            {
                MobType.Friendly => _friendlySpawnPoint.position,
                MobType.Enemy => _enemySpawnPoint.position,
                _ => throw new System.NotImplementedException()
            };
            mob.Direction = transform.localScale = mobType == MobType.Friendly ? Vector3.right : Vector3.left;
        }
    }
}

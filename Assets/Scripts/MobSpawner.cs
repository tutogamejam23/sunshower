using System.Collections.Generic;
using System.Linq;

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

        private void Start()
        {
            Debug.Assert(_friendlySpawnPoint, "Friendly Spawn Point가 연결되어 있지 않습니다!");
            Debug.Assert(_enemySpawnPoint, "Enemy Spawn Point가 연결되어 있지 않습니다!");

            foreach (var (id, mobData) in Stage.Instance.DataTable.MobTable)
            {
                _mobPool.Add(id, new EntityPool<Mob>(mobData, onRelease: OnReleaseFromPool));
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

            switch (mob.MobType)
            {
                case MobType.Friendly:
                    _activeFriendlyMobs.AddLast(mob);
                    break;
                case MobType.Enemy:
                    _activeEnemyMobs.AddLast(mob);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public void OnReleaseFromPool(Mob mob)
        {
            switch (mob.MobType)
            {
                case MobType.Friendly:
                    _activeFriendlyMobs.Remove(mob);
                    break;
                case MobType.Enemy:
                    _activeEnemyMobs.Remove(mob);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}

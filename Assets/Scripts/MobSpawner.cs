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

        private List<float> _friendlyRandomSpawnPointY;
        private List<float> _enemyRandomSpawnPointY;

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

            _friendlyRandomSpawnPointY = new List<float>();
            _enemyRandomSpawnPointY = new List<float>();
            for (var i = 1.2f; i >= 0f; i -= 0.3f)
            {
                _friendlyRandomSpawnPointY.Add(_friendlySpawnPoint.position.y + i);
                _enemyRandomSpawnPointY.Add(_enemySpawnPoint.position.y + i);
            }

            foreach (var (id, mobData) in Stage.Instance.DataTable.MobTable)
            {
                _mobPool.Add(id, new EntityPool<Mob>(mobData, onRelease: OnReleaseFromPool));
            }
        }

        public void Spawn(int mobID, EntitySideType side)
        {
            var mob = _mobPool[mobID].Get();

            switch (side)
            {
                case EntitySideType.Friendly:
                    {
                        var idx = _random.NextInt(0, _friendlyRandomSpawnPointY.Count * 100);
                        idx = math.min((int)math.round((float)idx / 100), _friendlyRandomSpawnPointY.Count - 1);
                        var y = _friendlyRandomSpawnPointY[idx];

                        mob.transform.position = new Vector3(_friendlySpawnPoint.position.x, y, _friendlySpawnPoint.position.z);
                        mob.transform.GetComponent<MeshRenderer>().sortingOrder = idx;
                        break;
                    }


                case EntitySideType.Enemy:
                    {
                        var idx = _random.NextInt(0, _enemyRandomSpawnPointY.Count * 100);
                        idx = math.min((int)math.round((float)idx / 100), _enemyRandomSpawnPointY.Count - 1);
                        var y = _enemyRandomSpawnPointY[idx];

                        mob.transform.position = new Vector3(_enemySpawnPoint.position.x, y, _enemySpawnPoint.position.z);
                        mob.transform.GetComponent<MeshRenderer>().sortingOrder = idx;
                        break;
                    }

                default:
                    throw new System.NotImplementedException();
            }

            // mob.HP = mob.Data.HP;
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

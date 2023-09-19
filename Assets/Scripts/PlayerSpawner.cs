using System;
using UnityEngine;

namespace Sunshower
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public Player ActivePlayer { get; private set; }

        public event EventHandler<Player> OnPlayerSpawned;

        private void Start()
        {
            Debug.Assert(_spawnPoint, "Spawn Point가 연결되어 있지 않습니다!");
        }

        public void Spawn(int playerID)
        {
            var data = Stage.Instance.DataTable.PlayerTable[playerID];
            var prefab = Resources.Load<GameObject>($"Prefabs/{data.PrefabPath}");
            var go = Instantiate(prefab);
            if (!go.TryGetComponent<Player>(out var player))
            {
                Debug.LogError($"{data.PrefabPath}에 Player 컴포넌트가 존재하지 않습니다!");
                return;
            }

            player.transform.position = _spawnPoint.position;
            player.Initialize(data);
            ActivePlayer = player;
            player.HP = data.HP;
            player.Direction = Vector3.right;
            OnPlayerSpawned?.Invoke(this, player);
        }
    }
}

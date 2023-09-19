using System;

using Unity.Collections;

using UnityEngine;

namespace Sunshower
{
    public class Stage : StateMachine<Stage>
    {
        [SerializeField] private DataTableManager _dataTable;
        [SerializeField] private PlayerSpawner _playerSpawner;
        [SerializeField] private MobSpawner _mobSpawner;
        [SerializeField, Min(1)] private int _stageNumber;

        public static Stage Instance { get; private set; }

        public DataTableManager DataTable => _dataTable;
        public Player ActivePlayer => _playerSpawner.ActivePlayer;
        public PlayerSpawner PlayerSpawner => _playerSpawner;
        public MobSpawner MobSpawner => _mobSpawner;

        public StageData StageInformation { get; private set; }
        public StageBeginState BeginState { get; private set; }
        public StageRunningState RunningState { get; private set; }
        public StageEndState EndState { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Assert(_dataTable, "DataTableManager 컴포넌트가 연결되어 있지 않습니다!");
            Debug.Assert(_playerSpawner, "PlayerSpawner 컴포넌트가 연결되어 있지 않습니다!");
            Debug.Assert(_mobSpawner, "MobSpawner 컴포넌트가 연결되어 있지 않습니다!");

            if (DataTable.StageTable.TryGetValue(_stageNumber, out var stageData))
            {
                StageInformation = stageData;
            }
            else
            {
                Debug.LogError($"{_stageNumber}번 스테이지 데이터가 존재하지 않습니다!");
                return;
            }

            BeginState = new StageBeginState();
            RunningState = new StageRunningState();
            EndState = new StageEndState();
            ChangeState(BeginState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

    public class StageBeginState : IState<Stage>
    {
        private float _timer;
        private float _currentTimer;

        public void Initialize()
        {
            _timer = 2;
            _currentTimer = 0;
        }

        public void Enter(Stage owner)
        {
            Initialize();
        }

        public void Execute(Stage owner)
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer >= _timer)
            {
                owner.ChangeState(owner.RunningState);
            }
        }

        public void Exit(Stage owner)
        {
        }
    }

    public class StageRunningState : IState<Stage>
    {
        private NativeQueue<int> _spawnQueue;
        private float _time;
        private float _spawnDelay;


        public void Initialize()
        {
        }

        public void Enter(Stage owner)
        {
            _spawnQueue = new NativeQueue<int>(Allocator.Persistent);
            Array.ForEach(owner.StageInformation.Spawner, id => _spawnQueue.Enqueue(id));
            _spawnDelay = owner.StageInformation.SpawnDelay;
            _time = 0f; // 첫번째 스폰 딜레이 한번 지난 후 생성

            owner.PlayerSpawner.Spawn(owner.StageInformation.PlayerID);
        }

        public void Execute(Stage owner)
        {
            if (_spawnQueue.Count > 0)
            {
                _time += Time.deltaTime;
                if (_time >= _spawnDelay)
                {
                    _time = 0f;
                    owner.MobSpawner.Spawn(_spawnQueue.Dequeue(), MobType.Enemy);
                }
            }
            else if (owner.MobSpawner.ActiveEnemyMobs.Count == 0)
            {
                owner.ChangeState(owner.EndState);
                return;
            }

            if (owner.LogEnabled)
            {
                Debug.Log($"Frendly Mobs: {owner.MobSpawner.ActiveFriendlyMobs.Count}, Enemy Mobs: {owner.MobSpawner.ActiveEnemyMobs.Count}");
            }
        }

        public void Exit(Stage owner)
        {
            _spawnQueue.Dispose();
        }
    }

    public class StageEndState : IState<Stage>
    {
        public void Initialize()
        {
        }

        public void Enter(Stage owner)
        {
            Debug.Log("Stage End");
        }

        public void Execute(Stage owner)
        {
            //owner.ChangeState()
        }

        public void Exit(Stage owner)
        {
        }
    }
}

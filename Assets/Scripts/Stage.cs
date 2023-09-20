using System;

using Unity.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunshower
{
    public class Stage : StateMachine<Stage>
    {
        [SerializeField] private DataTableManager _dataTable;
        [SerializeField] private ProjectileManager _projectileManager;
        [SerializeField] private EffectManager _effectManager;

        [SerializeField] private PlayerSpawner _playerSpawner;
        [SerializeField] private MobSpawner _mobSpawner;
        [SerializeField, Min(1)] private int _stageNumber;
        [SerializeField] private int _nextStageNumber = 0;

        public static Stage Instance { get; private set; }

        public DataTableManager DataTable => _dataTable;

        public ProjectileManager ProjectileManager => _projectileManager;
        public EffectManager EffectManager => _effectManager;

        public Player ActivePlayer => _playerSpawner.ActivePlayer;
        public PlayerSpawner PlayerSpawner => _playerSpawner;
        public MobSpawner MobSpawner => _mobSpawner;

        public int StageNumber => _stageNumber;
        public int NextStageNumber => _nextStageNumber;

        public StageData StageInformation { get; private set; }
        public StageBeginState BeginState { get; private set; }
        public StageRunningState RunningState { get; private set; }
        public StageEndState EndState { get; private set; }

        private void Awake()
        {
            SceneManager.LoadScene("StageUIScene", LoadSceneMode.Additive);

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
            Debug.Assert(_projectileManager, "ProjectileManager 컴포넌트가 연결되어 있지 않습니다!");
            Debug.Assert(_effectManager, "EffectManager 컴포넌트가 연결되어 있지 않습니다!");
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
            _timer = 0.1f; // 시작 딜레이
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
                    owner.MobSpawner.Spawn(_spawnQueue.Dequeue(), EntitySideType.Enemy);
                }
            }
            else if (owner.MobSpawner.ActiveEnemyMobs.Count == 0)
            {
                owner.EndState.IsFriendlyWin = true;
                owner.ChangeState(owner.EndState);
                return;
            }
            else if (owner.ActivePlayer.CurrentState == owner.ActivePlayer.PlayerDeadState)
            {
                owner.EndState.IsEnemyWin = true;
                owner.ChangeState(owner.EndState);
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
        public bool IsFriendlyWin { get; set; }
        public bool IsEnemyWin { get; set; }

        private float _time;

        public void Initialize()
        {
        }

        public void Enter(Stage owner)
        {
            Debug.Log("Stage End");

            if (_time >= 2f)
            {
                return;
            }
            _time += Time.deltaTime;


            if (_time < 2f)
            {
                return;
            }

            if (IsFriendlyWin)
            {
                if (owner.NextStageNumber > 0)
                {
                    SceneManager.LoadSceneAsync($"Stage{owner.NextStageNumber}Scene");
                }
                else
                {
                    SceneManager.LoadSceneAsync("EndingScene");
                }
            }
            else
            {
                // SceneManager.LoadSceneAsync("TitleScene");
            }
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

using System;
using System.Linq;

using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    public class Player : StateMachine<Player>, IGameEntity, ICostEntity, ISkillEntity
    {
        [field: SerializeField]
        public SkillManager SkillManager { get; private set; }

        public Transform Transform => transform;
        public Vector3 Direction { get; set; }
        public int ID => Info.ID;
        public EntitySideType EntitySide => EntitySideType.Friendly;
        public GameEntityData Info { get; private set; }

        public PlayerIdleState PlayerIdleState { get; private set; }
        public PlayerAttackState PlayerAttackState { get; private set; }
        public PlayerDeadState PlayerDeadState { get; private set; }

        public event EventHandler<(int previous, int current)> OnHPChanged;
        public event EventHandler<int> OnCostChanged;

        public int HP
        {
            get => _hp;
            set
            {
                if (_hp == 0 && CurrentState == PlayerDeadState)
                {
                    // 이미 체력이 0이 되어 DeadState로 변경됐음
                    return;
                }
                var prevHP = _hp;
                _hp = math.max(math.min(value, Info.HP), 0);
                if (prevHP != _hp)
                {
                    OnHPChanged?.Invoke(this, (prevHP, _hp));
                }

                if (_hp == 0)
                {
                    ChangeState(PlayerDeadState);
                }
            }
        }

        public int Cost
        {
            get => _cost;
            set
            {
                _cost = math.max(value, 0);
                OnCostChanged?.Invoke(this, _cost);
            }
        }

        private int _hp;
        private int _cost;
        private float _costUpTime;

        private void Awake()
        {
            Debug.Assert(SkillManager);

            PlayerIdleState = new PlayerIdleState();
            PlayerAttackState = new PlayerAttackState();
            PlayerDeadState = new PlayerDeadState();
        }

        private void OnEnable()
        {
            // Player는 풀링을 안할거라 Start에서 초기화해도 되긴 함
            PlayerIdleState.Initialize();
            PlayerAttackState.Initialize();
            PlayerDeadState.Initialize();

            ChangeState(PlayerIdleState);
        }

        protected override void Update()
        {
            base.Update();
            _costUpTime += Time.deltaTime;
            var playerInfo = Info as PlayerData;
            if (_costUpTime >= playerInfo.CostUpTime)
            {
                _costUpTime = 0f;
                Cost += 1;
            }
        }

        public void Initialize(GameEntityData data)
        {
            Info = data;
            SkillManager.Register(this, data.Skills);
        }
    }

    public class PlayerIdleState : IState<Player>
    {
        public void Initialize()
        {
        }

        public void Enter(Player owner)
        {
        }

        public void Execute(Player owner)
        {
        }

        public void Exit(Player owner)
        {
        }
    }

    public class PlayerAttackState : IState<Player>
    {
        public void Initialize()
        {
        }

        public void Enter(Player owner)
        {
        }

        public void Execute(Player owner)
        {
        }

        public void Exit(Player owner)
        {
        }
    }

    public class PlayerDeadState : IState<Player>
    {
        public void Initialize()
        {
        }

        public void Enter(Player owner)
        {
        }

        public void Execute(Player owner)
        {
            // 죽는 애니메이션 끝날 때 까지 대기
            UnityEngine.Object.Destroy(owner.gameObject);
        }

        public void Exit(Player owner)
        {
            // Execute에서 Destroy를 호출하므로 해당 메소드 실행 안됨!
        }
    }
}

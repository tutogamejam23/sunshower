using System.Linq;

using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    public enum EntityType
    {
        Player, Mob
    }

    public interface IGameEntity
    {
        public Transform Transform { get; }
        public Vector3 Direction { get; }
        public int ID { get; }
        public int HP { get; set; }
    }

    public interface ICostEntity
    {
        public int Cost { get; set; }
    }

    public class Player : StateMachine<Player>, IGameEntity, ICostEntity
    {
        [SerializeField] private SkillManager _skillManager;
        public ISkill<Player> Skill { get; set; } = null;

        public PlayerIdleState PlayerIdleState { get; private set; }
        public PlayerAttackState PlayerAttackState { get; private set; }
        public PlayerDeadState PlayerDeadState { get; private set; }

        public SkillManager SkillManager => _skillManager;
        public GameEntityData Data { get; private set; }

        public Transform Transform => transform;
        public Vector3 Direction { get; set; }
        public int ID => Data.ID;

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
                _hp = math.min(value, 0);
                if (_hp == 0)
                {
                    ChangeState(PlayerDeadState);
                }
            }
        }

        public int Cost { get => _cost; set => _cost = math.min(value, 0); }

        private int _hp;
        private int _cost;

        private void Awake()
        {
            Debug.Assert(_skillManager, "SkillManager 컴포넌트가 연결되어 있지 않습니다!");

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

        public void Initialize(GameEntityData data)
        {
            Data = data;
            SkillManager.Initialize(this, Data.Skills.Select(id => Stage.Instance.DataTable.SkillTable[id]));
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
            Object.Destroy(owner.gameObject);
        }

        public void Exit(Player owner)
        {
            // Execute에서 Destroy를 호출하므로 해당 메소드 실행 안됨!
        }
    }
}

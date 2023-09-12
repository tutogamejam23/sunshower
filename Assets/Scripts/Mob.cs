using System.Buffers;
using System.Linq;
using System.Text;
using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    public enum MobType
    {
        Friendly, Enemy
    }

    public class Mob : StateMachine<Mob>, IGameEntity, IEntityPoolItem<Mob>
    {
        [SerializeField] private SkillManager _skillManager;

        public MobMoveState MobMoveState { get; private set; }
        public MobAttackState MobAttackState { get; private set; }
        public MobDeadState MobDeadState { get; private set; }

        public SkillManager SkillManager => _skillManager;
        public GameEntityData Data { get; private set; }
        public IObjectPool<Mob> Pool { get; private set; }

        public MobType MobType { get; set; }
        public Vector3 Direction { get; set; }

        public Transform Transform => transform;

        public int ID => Data.ID;

        public int HP
        {
            get => _hp;
            set
            {
                if (_hp == 0 && CurrentState == MobDeadState)
                {
                    // 이미 체력이 0이 되어 DeadState로 변경됐음
                    return;
                }

                // TODO: 여기다 텍스쳐 깜빡이는 효과 넣기
                _hp = math.max(value, 0);
                if (_hp == 0)
                {
                    ChangeState(MobDeadState);
                }
            }
        }

        private int _hp;

        private void Awake()
        {
            Debug.Assert(_skillManager, "SkillManager 컴포넌트가 연결되어 있지 않습니다!");

            MobMoveState = new MobMoveState();
            MobAttackState = new MobAttackState();
            MobDeadState = new MobDeadState();
        }

        private void OnEnable()
        {
            // State를 재활용하기 위해 초기화
            MobMoveState.Initialize();
            MobAttackState.Initialize();
            MobDeadState.Initialize();

            ChangeState(MobMoveState);
        }

        public void Initialize(IObjectPool<Mob> pool, GameEntityData data)
        {
            Pool = pool;
            Data = data;
            SkillManager.Initialize(this, Data.Skills.Select(id => Stage.Instance.DataTable.SkillTable[id]));
        }
    }

    public class MobMoveState : IState<Mob>
    {
        public void Initialize()
        {
        }

        public void Enter(Mob owner)
        {
        }

        StringBuilder builder = new StringBuilder();

        public void Execute(Mob owner)
        {
            var range = -1f;
            foreach (var skill in owner.SkillManager.SkillMap.Values)
            {
                if (skill.Cooldown <= 0f && skill.Data.Range > 0f && skill.Data.Range > range)
                {
                    range = skill.Data.Range;
                }
            }


            // 가지고 있는 스킬 중 가장 공격범위가 먼 스킬을 기준으로 공격범위 내에 공격 대상이 있는지 확인
            var nearestEnties = ArrayPool<IGameEntity>.Shared.Rent(30);
            try
            {
                var nearestCount = Skill.GetNearestEntity(owner, range, ref nearestEnties);
                if (nearestCount > 0)
                {
                    if (owner.LogEnabled)
                    {
                        builder.Clear();
                        builder.Append(Time.time).Append(": ");
                        for (int i = 0; i < nearestCount; i++)
                        {
                            builder.Append(nearestEnties[i].ID).Append(", ");
                        }
                        Debug.Log(builder.ToString());
                    }

                    for (int i = 0; i < nearestCount; i++)
                    {
                        var entity = nearestEnties[i];
                        if (entity is Player)
                        {
                            owner.ChangeState(owner.MobAttackState);
                            return;
                        }
                        else if (entity is Mob mob && mob.MobType != owner.MobType)
                        {
                            owner.ChangeState(owner.MobAttackState);
                            return;
                        }
                    }
                }
            }
            finally
            {
                // Rent 했으면 무조건 Return해줘야 Memory Leak이 안생김
                ArrayPool<IGameEntity>.Shared.Return(nearestEnties);
            }
            if (range < 0f)
            {
                return;
            }

            owner.transform.position += 0.1f * owner.Data.Speed * Time.deltaTime * owner.Direction;
        }

        public void Exit(Mob owner)
        {
        }
    }

    public class MobAttackState : IState<Mob>
    {
        private Skill _usedSkill;

        public void Initialize()
        {
        }

        public void Enter(Mob owner)
        {
            _usedSkill = null;
            foreach (var skill in owner.SkillManager.SkillMap.Values)
            {
                if (skill.Use())
                {
                    _usedSkill = skill;
                    break;
                }
            }
            if (_usedSkill is null)
            {
                // 사용할 수 있는 스킬이 없으면 다시 MoveState로 변경
                owner.ChangeState(owner.MobMoveState);
            }
        }

        public void Execute(Mob owner)
        {
            if (_usedSkill.DelayTime <= 0f)
            {
                // 딜레이가 끝나면 다시 MoveState로 변경
                owner.ChangeState(owner.MobMoveState);
            }
        }

        public void Exit(Mob owner)
        {
        }
    }

    public class MobDeadState : IState<Mob>
    {
        public void Initialize()
        {
        }

        public void Enter(Mob owner)
        {
        }

        public void Execute(Mob owner)
        {
            // 죽는 애니메이션 끝날 때 까지 대기
            owner.Pool.Release(owner);
        }

        public void Exit(Mob owner)
        {
            // Execute에서 Pool.Release를 호출하므로 해당 메소드 실행 안됨!
        }
    }
}

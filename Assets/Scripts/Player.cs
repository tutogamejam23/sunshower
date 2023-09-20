using System;
using System.Linq;
using DG.Tweening;
using Spine;
using Spine.Unity;
using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    public class Player : StateMachine<Player>, IGameEntity, ICostEntity, ISkillEntity
    {
        [field: SerializeField]
        public SkillManager SkillManager { get; private set; }

        [field: SerializeField]
        public SkeletonAnimation Animation { get; private set; }

        public Transform Transform => transform;
        public Vector3 Direction { get; set; }
        public int ID => Data.ID;
        public EntitySideType EntitySide => EntitySideType.Friendly;
        public GameEntityData Data { get; private set; }
        public PlayerData PlayerData => Data as PlayerData;

        public PlayerIdleState PlayerIdleState { get; private set; }
        public PlayerSkillState PlayerSkillState { get; private set; }
        public PlayerDeadState PlayerDeadState { get; private set; }

        public event EventHandler<(int previous, int current)> OnHPChanged;
        public event EventHandler<int> OnCostChanged;

        private Tween _hitTween;

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
                var next = math.max(math.min(value, Data.HP), 0);

                var diff = next - _hp;
                _hp = next;

                if (diff < 0)
                {
                    if (_hitTween != null && _hitTween.IsPlaying())
                    {
                        _hitTween.Complete();
                    }
                    _hitTween = transform.DOPunchPosition(Direction * 0.1f, 0.2f);
                }

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
                var data = Data as PlayerData;
                var nextCost = math.max(math.min(value, data.MaxCost), 0);
                if (_cost != nextCost)
                {
                    _cost = nextCost;
                    OnCostChanged?.Invoke(this, _cost);
                }
            }
        }

        private int _hp;
        private int _cost;
        private float _costUpTime;

        private void Awake()
        {
            Debug.Assert(SkillManager);
            Debug.Assert(Animation);

            PlayerIdleState = new PlayerIdleState();
            PlayerSkillState = new PlayerSkillState();
            PlayerDeadState = new PlayerDeadState();
        }

        private void OnEnable()
        {
            // Player는 풀링을 안할거라 Start에서 초기화해도 되긴 함
            PlayerIdleState.Initialize();
            PlayerSkillState.Initialize();
            PlayerDeadState.Initialize();
        }

        protected override void Update()
        {
            base.Update();
            _costUpTime += Time.deltaTime;
            var playerInfo = Data as PlayerData;
            if (_costUpTime >= playerInfo.CostUpTime)
            {
                _costUpTime = 0f;
                Cost += 1;
            }
        }

        public void Initialize(GameEntityData data)
        {
            Data = data;
            SkillManager.Register(this, data.Skills);
        }

        public void OnActive()
        {
            ChangeState(PlayerIdleState);
        }

        public void ExecuteSkill(int skillID)
        {
            var skill = SkillManager.GetSkill(skillID);
            if (skill == null)
            {
                return;
            }

            if (!skill.CanUse())
            {
                return;
            }

            PlayerSkillState.Skill = skill;
            ChangeState(PlayerSkillState);
        }
    }

    public class PlayerIdleState : IState<Player>
    {
        public void Initialize()
        {
        }

        public void Enter(Player owner)
        {
            owner.Animation.AnimationState.SetAnimation(0, owner.PlayerData.IdleAnimation, true);
        }

        public void Execute(Player owner)
        {
        }

        public void Exit(Player owner)
        {
        }
    }

    public class PlayerSkillState : IState<Player>
    {
        public Skill Skill { get; set; }

        private Player _owner;

        public void Initialize()
        {
        }

        public void Enter(Player owner)
        {
            owner.Animation.AnimationState.End += OnAnimationEnd;
            _owner = owner;

            var executed = Skill.Execute();
            if (!executed)
            {
                owner.ChangeState(owner.PlayerIdleState);
            }
        }

        public void Execute(Player owner)
        {
        }

        public void Exit(Player owner)
        {
            owner.Animation.AnimationState.End -= OnAnimationEnd;
        }

        private void OnAnimationEnd(TrackEntry trackEntry)
        {
            _owner.ChangeState(_owner.PlayerIdleState);
        }
    }

    public class PlayerDeadState : IState<Player>
    {
        public void Initialize()
        {
        }

        public void Enter(Player owner)
        {
            owner.Animation.AnimationState.SetAnimation(0, owner.PlayerData.DeadAnimation, false);
            SoundManager.instance.PlaySFXAtPosition(owner.PlayerData.DeadSFX, owner.transform.position);
        }

        public void Execute(Player owner)
        {
            // 죽는 애니메이션 끝날 때 까지 대기
            // UnityEngine.Object.Destroy(owner.gameObject);
        }

        public void Exit(Player owner)
        {
            // Execute에서 Destroy를 호출하므로 해당 메소드 실행 안됨!
        }
    }
}

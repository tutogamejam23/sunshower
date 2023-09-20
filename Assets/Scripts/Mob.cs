using DG.Tweening;

using Spine;
using Spine.Unity;

using System;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    public class Mob : StateMachine<Mob>, IGameEntity, IEntityPoolItem<Mob>, ISkillEntity
    {
        [field: SerializeField]
        public SkillManager SkillManager { get; private set; }

        [field: SerializeField]
        public SkeletonAnimation Animation { get; private set; }

        [field: SerializeField, SpineSkin]
        public string DefaultSkin { get; private set; }

        [field: SerializeField, SpineSkin]
        public string CharmingSkin { get; private set; }

        public MobMoveState MobMoveState { get; private set; }
        public MobAttackState MobAttackState { get; private set; }
        public MobDeadState MobDeadState { get; private set; }

        public GameEntityData Data { get; private set; }
        public MobData MobData => Data as MobData;
        public IObjectPool<Mob> Pool { get; private set; }
        public Vector3 Direction { get; set; }
        public EntitySideType EntitySide { get; set; }

        public Transform Transform => transform;

        public int ID => Data.ID;

        // private Tween _hitTween;
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
                var next = math.max(math.min(value, Data.HP), 0);
                var diff = next - _hp;
                _hp = next;

                if (diff < 0)
                {
                    Animation.GetComponent<Renderer>().SetPropertyBlock(_materialProp);
                    Animation.transform.DOShakeScale(0.15f, 0.05f);
                    _hitTime = 0f;
                    _matChanged = true;
                }

                if (_hp == 0)
                {
                    ChangeState(MobDeadState);
                }
            }
        }

        private int _hp;
        private float _hitTime;
        private bool _matChanged;
        private MaterialPropertyBlock _materialProp;

        private void Awake()
        {
            Debug.Assert(SkillManager);
            Debug.Assert(Animation);

            MobMoveState = new MobMoveState();
            MobAttackState = new MobAttackState();
            MobDeadState = new MobDeadState();

            _materialProp = new MaterialPropertyBlock();
            _materialProp.SetColor("_Color", Color.red);

            // _random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        }

        private void OnEnable()
        {
            // State를 재활용하기 위해 초기화
            MobMoveState.Initialize();
            MobAttackState.Initialize();
            MobDeadState.Initialize();
        }

        protected override void Update()
        {
            base.Update();

            if (_hitTime < 0.15f)
            {
                _hitTime += Time.deltaTime;
            }
            else if(_matChanged)
            {
                Animation.GetComponent<Renderer>().SetPropertyBlock(null);
                _matChanged = false;
            }
        }

        public void OnActive()
        {
            _hp = Data.HP;
            SkillManager.Clear();
            ChangeState(MobMoveState);
        }

        public void Initialize(IObjectPool<Mob> pool, GameEntityData data)
        {
            Pool = pool;
            Data = data;
            SkillManager.Register(this, data.Skills);
        }
    }

    public class MobMoveState : IState<Mob>
    {
        private ArrayPool<IGameEntity> _entityPool;

        public void Initialize()
        {
        }

        public void Enter(Mob owner)
        {
            _entityPool = new ArrayPool<IGameEntity>(30);

            // Move animation
            var entry = owner.Animation.AnimationState.SetAnimation(0, owner.MobData.MoveAnimation, true);
            entry.Complete += OnAnimationComplete;
        }

        public void Execute(Mob owner)
        {
            var mobData = owner.Data as MobData;
            var range = mobData.Range;
            var span = _entityPool.Span;

            var isCharming = owner.SkillManager.IsCharming > 0;
            var direction = isCharming ? owner.Direction * -1f : owner.Direction;
            var side = owner.EntitySide switch
            {
                EntitySideType.Friendly => EntitySideType.Enemy,
                EntitySideType.Enemy => EntitySideType.Friendly,
                _ => throw new NotImplementedException()
            }; // 매혹일 때는 스킬매니저에서 이미 처리함
            var nearestCount = Skill.GetNearestEntities(owner, direction, range, ref span, filter: side);
            // var nearestEntities = span[..nearestCount];


            // 몹 공격범위에 공격 가능한 대상이 있으면 공격
            if (nearestCount > 0)
            {
                owner.ChangeState(owner.MobAttackState);
                return;
            }

            // 매혹에 걸린 경우 반대 방향으로 이동

            if (owner.EntitySide == EntitySideType.Friendly && owner.transform.position.x >= Stage.Instance.MapEndX)
            {
                return;
            }

            if (Stage.Instance.CurrentState == Stage.Instance.EndState)
            {
                return;
            }
            var speedRate = owner.SkillManager.SpeedRateBuffAverage;
            owner.transform.position += 0.1f * owner.Data.Speed * speedRate * Time.deltaTime * direction;
        }

        public void Exit(Mob owner)
        {
            _entityPool.Dispose();
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            // play walk sound
            // SoundManager.instance.PlaySFXAtPosition(_owner.MobData.MoveSFX, _owner.transform.position);
        }
    }

    public class MobAttackState : IState<Mob>
    {
        private ArrayPool<IGameEntity> _entityPool;

        public void Initialize()
        {
        }

        public void Enter(Mob owner)
        {
            _entityPool = new ArrayPool<IGameEntity>(30);
        }

        public void Execute(Mob owner)
        {
            var mobData = owner.Data as MobData;
            var range = mobData.Range;
            var span = _entityPool.Span;

            var isCharming = owner.SkillManager.IsCharming > 0;
            var direction = isCharming ? owner.Direction * -1f : owner.Direction;
            var side = owner.EntitySide switch
            {
                EntitySideType.Friendly => EntitySideType.Enemy,
                EntitySideType.Enemy => EntitySideType.Friendly,
                _ => throw new NotImplementedException()
            };
            var nearestCount = Skill.GetNearestEntities(owner, direction, range, ref span, filter: side);
            if (nearestCount == 0)
            {
                owner.ChangeState(owner.MobMoveState);
                return;
            }

            if (owner.SkillManager.Delay > 0f)
            {
                // 딜레이 끝날 때 까지 대기
                return;
            }

            var skills = owner.SkillManager.Skills;
            // 보통 쿨타임이 긴 스킬이 뒤에 있기 때문에 뒤에서부터 체크
            for (int i = skills.Count - 1; i >= 0; i--)
            {
                var skill = skills[i];
                if (skill.CanUse())
                {
                    skill.Execute();
                    break;
                }
            }
        }

        public void Exit(Mob owner)
        {
            _entityPool.Dispose();
        }
    }

    public class MobDeadState : IState<Mob>
    {
        private Mob _owner;

        public void Initialize()
        {
        }

        public void Enter(Mob owner)
        {
            _owner = owner;
            var entry = owner.Animation.AnimationState.SetAnimation(0, owner.MobData.DeadAnimation, false);
            SoundManager.instance.PlaySFXAtPosition(owner.MobData.DeadSFX, owner.transform.position);
            entry.Complete += OnAnimationComplete;
        }

        public void Execute(Mob owner)
        {
            // 죽는 애니메이션 끝날 때 까지 대기

        }

        public void Exit(Mob owner)
        {
            // Execute에서 Pool.Release를 호출하므로 해당 메소드 실행 안됨!
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            trackEntry.Complete -= OnAnimationComplete;
            _owner.Pool.Release(_owner);
        }
    }
}

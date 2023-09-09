using Unity.Entities;

namespace Sunshower
{
    public readonly partial struct MobStateAspect : IAspect
    {
        public readonly Entity Entity;
        public readonly RefRW<MobComponent> Mob;
        public readonly RefRW<MobMoveStateComponent> MoveState;
        public readonly RefRW<MobAttackStateComponent> AttackState;
        public readonly RefRW<MobDeadStateComponent> DeadState;
    }

    public struct MobMoveStateComponent : IStateComponent
    {
        public float Direction;
        public float Speed;
        public float AttackRange;
    }

    public struct MobAttackStateComponent : IStateComponent
    {
        public int SkillID;
        public float StartTime;
        public float Delay;
        public float Time;
    }

    public struct MobDeadStateComponent : IStateComponent
    {
        public float StartTime;
        public float Delay;
        public float Time;
    }
}

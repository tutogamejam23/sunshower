using Unity.Entities;
using Unity.Transforms;

namespace Sunshower
{
    public enum MobType
    {
        Freindly = 1,
        Enemy = 2
    }

    public enum MobState
    {
        Move,
        Attack,
        Dead
    }

    public struct MobComponent : IComponentData
    {
        public Entity Entity;
        public MobType Type;
        public MobState State;
    }

    public readonly partial struct MobAspect : IAspect
    {
        public readonly Entity Entity;
        public readonly RefRW<LocalTransform> Transform;
        public readonly RefRW<GameEntityComponent> GameEntity;
        public readonly RefRW<AnimatorComponent> Animator;
        public readonly RefRW<MobComponent> Mob;
        public readonly RefRO<GameEntityDataComponent> Data;
    }
}

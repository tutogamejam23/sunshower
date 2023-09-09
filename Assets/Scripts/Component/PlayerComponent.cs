using Unity.Entities;

namespace Sunshower
{
    public struct PlayerComponent : IComponentData
    {
        public Entity Entity;
    }

    public readonly partial struct PlayerAspect : IAspect
    {
        public readonly Entity Entity;
        public readonly RefRW<GameEntityComponent> GameEntity;
        public readonly RefRW<PlayerComponent> Player;
        public readonly RefRW<SkillComponent> Skill;
    }
}

using Unity.Entities;

namespace Sunshower
{
    public partial struct SkillSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SkillComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
        }
    }
}

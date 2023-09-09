using Unity.Entities;

namespace Sunshower
{
    public struct SkillComponent : IComponentData
    {
    }

    [InternalBufferCapacity(4)]
    public struct SkillBufferElement : IBufferElementData
    {
        public BlobAssetReference<SkillData> Data;
        public float Cooldown;
    }
}

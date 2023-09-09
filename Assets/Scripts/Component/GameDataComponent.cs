using Unity.Entities;

namespace Sunshower
{
    public struct GameDataComponent : IComponentData
    {
        public Entity Entity;
    }

    public readonly struct GameEntityDataComponent : IComponentData
    {
        public readonly BlobAssetReference<GameEntityData> Data;

        public GameEntityDataComponent(BlobAssetReference<GameEntityData> data)
        {
            Data = data;
        }
    }

    public readonly struct SkillDataComponent : IComponentData
    {
        public readonly BlobAssetReference<SkillData> Data;

        public SkillDataComponent(BlobAssetReference<SkillData> data)
        {
            Data = data;
        }
    }
}

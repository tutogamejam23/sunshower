using Unity.Entities;

namespace Sunshower
{
    public readonly struct PlayerDataComponent : IComponentData
    {
        public BlobAssetReference<PlayerData> PlayerData { get; }

        public PlayerDataComponent(BlobAssetReference<PlayerData> data)
        {
            PlayerData = data;
        }
    }

    public readonly struct MobDataPoolComponent : IComponentData
    {
        public BlobAssetReference<MobDataPool> Pool { get; }

        public MobDataPoolComponent(BlobAssetReference<MobDataPool> data)
        {
            Pool = data;
        }
    }

    public readonly struct SkillDataPoolComponent : IComponentData
    {
        public BlobAssetReference<SkillDataPool> Pool { get; }

        public SkillDataPoolComponent(BlobAssetReference<SkillDataPool> data)
        {
            Pool = data;
        }
    }

    /// <summary>
    /// 플레이어 데이터
    /// </summary>
    public struct PlayerData
    {
        public int ID;
        public BlobString Name;
        public int HP;
        public BlobArray<int> Skills;
    }

    /// <summary>
    /// 몹 데이터
    /// </summary>
    public struct MobData
    {
        public int ID;
        public BlobString Name;
        public int HP;
        public float Speed;
        public BlobArray<int> Skills;
    }

    public struct MobDataPool
    {
        public BlobArray<MobData> Mobs;
    }

    /// <summary>
    /// 스킬 데이터
    /// </summary>
    public struct SkillData
    {
        public int ID;
        public BlobString Name;
        public BlobString Description;
        public int Cost;
        public float Cooldown;
        public int Damage;
        public float Range;
        public float Duration;
        public int HealAmount;
        public float SpeedDecreaseRate;
        public float SpeedDecreaseTime;
        public BlobArray<int> SpawnMobIDs;
        public BlobArray<int> SpawnMobCounts;
    }

    public struct SkillDataPool
    {
        public BlobArray<SkillData> Skills;
    }
}

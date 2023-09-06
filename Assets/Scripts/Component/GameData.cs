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
        /*
        id: integer
        name: string
        hp: integer
        skills: array<integer>
        */
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
        /*
        id: integer
        name: string
        hp: integer
        speed: float
        skills: array<integer>
        */

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
        /*
        id: integer
        name: string
        description: string
        cost: integer
        cooldown: float
        damage: integer
        range: float
        duration: float
        speed_decrease_rate: float
        speed_decrease_time: float
        spawn_mob_id: array<integer>
        spawn_mob_count: array<integer>
         */
        public int ID;
        public BlobString Name;
        public BlobString Description;
        public int Cost;
        public float CoolDown;
        public int Damage;
        public float Range;
        public float Duration;
        public float SpeedDecreaseRate;
        public float SpeedDecreaseTime;
        public BlobArray<int> SpawnMobID;
        public BlobArray<int> SpawnMobCount;
    }

    public struct SkillDataPool
    {
        public BlobArray<SkillData> Skills;
    }
}

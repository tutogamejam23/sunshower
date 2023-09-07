using System;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

using YamlDotNet.Serialization;

namespace Sunshower
{
    [BakingType]
    public class GameDataAuthoring : MonoBehaviour
    {
        public static readonly string PlayerTablePath = "DataTable/PlayerTable";
        public static readonly string MobTablePath = "DataTable/MobTable";
        public static readonly string SkillTablePath = "DataTable/SkillTable";

        private static readonly IDeserializer DataTableDeserializer =
            new DeserializerBuilder()
            //.IgnoreUnmatchedProperties()
            .Build();

        private class Baker : Baker<GameDataAuthoring>
        {
            public override void Bake(GameDataAuthoring authoring)
            {
                BlobAssetReference<PlayerData> playerBlobRef = LoadPlayerTable();
                BlobAssetReference<MobDataPool> mobsBlobRef = LoadMobTable();
                BlobAssetReference<SkillDataPool> skillsBlobRef = LoadSkilTable();

                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerDataComponent(playerBlobRef));
                AddComponent(entity, new MobDataPoolComponent(mobsBlobRef));
                AddComponent(entity, new SkillDataPoolComponent(skillsBlobRef));
            }
        }

        private class PlayerDataRaw
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int HP { get; set; }
            public int[] Skills { get; set; } = Array.Empty<int>();
        }

        private class MobDataRaw
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int HP { get; set; }
            public float Speed { get; set; }
            public int[] Skills { get; set; } = Array.Empty<int>();
        }

        private class SkillDataRaw
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Cost { get; set; }
            public float Cooldown { get; set; }
            public int Damage { get; set; }
            public float Range { get; set; }
            public float Duration { get; set; }
            public int HealAmount { get; set; }
            public float SpeedDecreaseRate { get; set; }
            public float SpeedDecreaseTime { get; set; }
            public int[] SpawnMobIDs { get; set; } = Array.Empty<int>();
            public int[] SpawnMobCounts { get; set; } = Array.Empty<int>();
        }

        private static BlobAssetReference<PlayerData> LoadPlayerTable()
        {
            var asset = Resources.Load<TextAsset>(PlayerTablePath);
            var table = DataTableDeserializer.Deserialize<PlayerDataRaw>(asset.text);
            var builder = new BlobBuilder(Allocator.Temp);
            try
            {
                ref var playerData = ref builder.ConstructRoot<PlayerData>();
                playerData = new PlayerData();
                ConvertPlayerData(ref builder, table, ref playerData);

                return builder.CreateBlobAssetReference<PlayerData>(Allocator.Persistent);
            }
            finally
            {
                builder.Dispose();
            }
        }

        private static void ConvertPlayerData(ref BlobBuilder builder, PlayerDataRaw data, ref PlayerData player)
        {
            player.ID = data.ID;
            builder.AllocateString(ref player.Name, data.Name.ToString());
            player.HP = data.HP;
            var skills = builder.Allocate(ref player.Skills, data.Skills.Length);
            for (int i = 0; i < skills.Length; i++)
            {
                skills[i] = data.Skills[i];
            }
        }

        private static BlobAssetReference<MobDataPool> LoadMobTable()
        {
            var asset = Resources.Load<TextAsset>(MobTablePath);
            var table = DataTableDeserializer.Deserialize<List<MobDataRaw>>(asset.text);
            var builder = new BlobBuilder(Allocator.Temp);
            try
            {
                ref var pool = ref builder.ConstructRoot<MobDataPool>();
                var mobs = builder.Allocate(ref pool.Mobs, table.Count);
                ConvertMobData(ref builder, table.ToArray(), ref mobs);

                return builder.CreateBlobAssetReference<MobDataPool>(Allocator.Persistent);
            }
            finally
            {
                builder.Dispose();
            }
        }

        private static void ConvertMobData(ref BlobBuilder builder, MobDataRaw[] datas, ref BlobBuilderArray<MobData> mobs)
        {
            Debug.Assert(datas.Length == mobs.Length);

            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                ref var mob = ref mobs[i];
                mob.ID = data.ID;
                builder.AllocateString(ref mob.Name, data.Name.ToString());
                mob.HP = data.HP;
                mob.Speed = data.Speed;
                var skills = builder.Allocate(ref mob.Skills, data.Skills.Length);
                for (int j = 0; j < skills.Length; j++)
                {
                    skills[j] = data.Skills[j];
                }
            }
        }

        private static BlobAssetReference<SkillDataPool> LoadSkilTable()
        {
            var asset = Resources.Load<TextAsset>(SkillTablePath);
            var table = DataTableDeserializer.Deserialize<SkillDataRaw[]>(asset.text);
            var builder = new BlobBuilder(Allocator.Temp);
            try
            {
                ref var pool = ref builder.ConstructRoot<SkillDataPool>();
                var skills = builder.Allocate(ref pool.Skills, table.Length);
                ConvertSkillData(ref builder, table, ref skills);

                return builder.CreateBlobAssetReference<SkillDataPool>(Allocator.Persistent);
            }
            finally
            {
                builder.Dispose();
            }
        }

        private static void ConvertSkillData(ref BlobBuilder builder, SkillDataRaw[] datas, ref BlobBuilderArray<SkillData> skills)
        {
            Debug.Assert(datas.Length == skills.Length);

            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                ref var skill = ref skills[i];
                skill.ID = data.ID;
                builder.AllocateString(ref skill.Name, data.Name.ToString());
                builder.AllocateString(ref skill.Description, data.Description.ToString());
                skill.Cost = data.Cost;
                skill.Cooldown = data.Cooldown;
                skill.Damage = data.Damage;
                skill.Range = data.Range;
                skill.Duration = data.Duration;
                skill.SpeedDecreaseRate = data.SpeedDecreaseRate;
                skill.SpeedDecreaseTime = data.SpeedDecreaseTime;
                var spawnMobIDs = builder.Allocate(ref skill.SpawnMobIDs, data.SpawnMobIDs.Length);
                for (int j = 0; j < spawnMobIDs.Length; j++)
                {
                    spawnMobIDs[j] = data.SpawnMobIDs[j];
                }
                var spawnMobCounts = builder.Allocate(ref skill.SpawnMobCounts, data.SpawnMobCounts.Length);
                for (int j = 0; j < spawnMobCounts.Length; j++)
                {
                    spawnMobCounts[j] = data.SpawnMobCounts[j];
                }
            }
        }
    }
}
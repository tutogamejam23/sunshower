using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;

using UnityEngine;

using YamlDotNet.Serialization;

using EntityDataBufferElement = Sunshower.GameDataBufferElement<Sunshower.GameEntityData>;
using SkillDataBufferElement = Sunshower.GameDataBufferElement<Sunshower.SkillData>;

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
            .Build();

        private class Baker : Baker<GameDataAuthoring>
        {
            public override void Bake(GameDataAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameDataComponent { Entity = entity });

                // 플레이어든 몹이든 전부 GameEntity 취급이며 이를 구분하기 위해 EntityType을 사용한다.
                var entityDataBuffer = AddBuffer<EntityDataBufferElement>(entity);
                ImportTableToBuffer<GameEntityDataRaw, GameEntityData>(PlayerTablePath, ref entityDataBuffer, ConvertPlayerData);
                ImportTableToBuffer<GameEntityDataRaw, GameEntityData>(MobTablePath, ref entityDataBuffer, ConvertMobData);
                var entityDataList = entityDataBuffer.ToNativeArray(Allocator.Temp);

                // Skill은 다른 유형이므로 따로 처리한다.
                var skillDataBuffer = AddBuffer<SkillDataBufferElement>(entity);
                ImportTableToBuffer<SkillDataRaw, SkillData>(SkillTablePath, ref skillDataBuffer, ConvertSkillData);
                using var skillMap = new NativeHashMap<int, BlobAssetReference<SkillData>>(skillDataBuffer.Length, Allocator.Temp);
                foreach (var asset in skillDataBuffer)
                {
                    ref var data = ref asset.Data.Value;
                    skillMap.Add(data.ID, asset.Data);
                }

                // Register Player Prefabs
                var playerPrefabBuffer = AddBuffer<PlayerPrefabBufferElement>(entity);
                foreach (var elem in entityDataList)
                {
                    ref var blobref = ref elem.Data.Value;
                    if (blobref.EntityType != GameEntityType.Player)
                    {
                        continue;
                    }
                    var (prefab, animator) = GetEntityPrefab(blobref.Prefab.GetUTF8String());
                    playerPrefabBuffer.Add(new PlayerPrefabBufferElement(blobref.ID, prefab, elem.Data, animator));
                    var buffer = AddBuffer<SkillBufferElement>(prefab);
                    foreach (var id in blobref.Skills.ToArray())
                    {
                        buffer.Add(new SkillBufferElement { Data = skillMap[id] });
                    }
                }

                // Register Mob Prefabs
                var mobPrefabBuffer = AddBuffer<MobPrefabBufferElement>(entity);
                foreach (var elem in entityDataList)
                {
                    ref var blobref = ref elem.Data.Value;
                    if (blobref.EntityType != GameEntityType.Mob)
                    {
                        continue;
                    }
                    var (prefab, animator) = GetEntityPrefab(blobref.Prefab.GetUTF8String());
                    mobPrefabBuffer.Add(new MobPrefabBufferElement(blobref.ID, prefab, elem.Data, animator));
                    var buffer = AddBuffer<SkillBufferElement>(prefab);
                    foreach (var id in blobref.Skills.ToArray())
                    {
                        buffer.Add(new SkillBufferElement { Data = skillMap[id] });
                    }
                }
            }

            private (Entity prefab, WeakObjectReference<Animator> animator) GetEntityPrefab(in string prefabData)
            {
                var prefabLocation = $"Prefabs/{prefabData}";
                var prefab = Resources.Load<GameObject>(prefabLocation);
                if (!prefab)
                {
                    Debug.LogWarning("[GameDataAuthoring] Failed to load prefab: " + prefabLocation);
                    return (Entity.Null, default);
                }
                var entityPrefab = GetEntity(prefab, TransformUsageFlags.Dynamic);
                var animator = prefab.GetComponent<Animator>();
                return (entityPrefab, animator ? new WeakObjectReference<Animator>(animator) : default);
            }

            // NOTE: 시간을 갈아넣어 만든 코드
            private delegate void TableConverter<RawT, BlobT>(ref BlobBuilder builder, RawT source, ref BlobT dest)
                where RawT : class
                where BlobT : unmanaged, IBlobData;

            private static void ImportTableToBuffer<RawT, BlobT>(string tablePath, ref DynamicBuffer<GameDataBufferElement<BlobT>> buffer, TableConverter<RawT, BlobT> converter)
                where RawT : class
                where BlobT : unmanaged, IBlobData
            {
                var asset = Resources.Load<TextAsset>(tablePath);
                var table = DataTableDeserializer.Deserialize<RawT[]>(asset.text);

                foreach (var item in table)
                {
                    var builder = new BlobBuilder(Allocator.Temp);
                    try
                    {
                        ref var data = ref builder.ConstructRoot<BlobT>();
                        converter(ref builder, item, ref data);
                        var assetRef = builder.CreateBlobAssetReference<BlobT>(Allocator.Persistent);
                        buffer.Add((GameDataBufferElement<BlobT>)assetRef);
                    }
                    finally
                    {
                        builder.Dispose();
                    }
                }
            }

            private static void ConvertGameEntityData(ref BlobBuilder builder, GameEntityDataRaw data, ref GameEntityData gameEntity)
            {
                gameEntity.EntityType = GameEntityType.None;
                gameEntity.ID = data.ID;
                gameEntity.Name.InitUTF8String(ref builder, data.Name);
                gameEntity.Prefab.InitUTF8String(ref builder, data.Prefab);
                gameEntity.HP = data.HP;
                gameEntity.Speed = data.Speed;
                gameEntity.Skills.Init(ref builder, data.Skills);
            }

            private static void ConvertPlayerData(ref BlobBuilder builder, GameEntityDataRaw data, ref GameEntityData player)
            {
                ConvertGameEntityData(ref builder, data, ref player);
                player.EntityType = GameEntityType.Player;
            }

            private static void ConvertMobData(ref BlobBuilder builder, GameEntityDataRaw data, ref GameEntityData mob)
            {
                ConvertGameEntityData(ref builder, data, ref mob);
                mob.EntityType = GameEntityType.Mob;
            }

            private static void ConvertSkillData(ref BlobBuilder builder, SkillDataRaw data, ref SkillData skill)
            {
                skill.ID = data.ID;
                skill.Name.InitUTF8String(ref builder, data.Name);
                skill.Description.InitUTF8String(ref builder, data.Description);
                skill.Animation.InitUTF8String(ref builder, data.Animation);
                skill.Cost = data.Cost;
                skill.Delay = data.Delay;
                skill.Cooldown = data.Cooldown;
                skill.Damage = data.Damage;
                skill.Range = data.Range;
                skill.Duration = data.Duration;
                skill.HealAmount = data.HealAmount;
                skill.SpeedDecreaseRate = data.SpeedDecreaseRate;
                skill.SpeedDecreaseTime = data.SpeedDecreaseTime;
                skill.SpawnMobIDs.Init(ref builder, data.SpawnMobIDs);
                skill.SpawnMobCounts.Init(ref builder, data.SpawnMobCounts);
            }
        }
    }
}

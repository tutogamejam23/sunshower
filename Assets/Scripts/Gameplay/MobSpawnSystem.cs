using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using UnityEngine;

namespace Sunshower
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct MobSpawnSystem : ISystem, ISystemStartStop
    {
        private NativeHashMap<int, MobPrefabBufferElement> _mobPrefabs;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MobSpawnerComponent>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnStartRunning(ref SystemState state)
        {
            _mobPrefabs = new NativeHashMap<int, MobPrefabBufferElement>(64, Allocator.Persistent);

            var prefabBuffer = SystemAPI.GetSingletonBuffer<MobPrefabBufferElement>(isReadOnly: true);
            Debug.Assert(prefabBuffer.IsCreated);

            foreach (var prefab in prefabBuffer)
            {
                _mobPrefabs.Add(prefab.ID, prefab);
            }
        }

        public void OnStopRunning(ref SystemState state)
        {
            _mobPrefabs.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            var buffer = SystemAPI.GetSingletonBuffer<MobSpawnBufferElement>();
            if (buffer.IsEmpty)
            {
                return;
            }

            var ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var mobSpawnerEntity = SystemAPI.GetSingletonEntity<MobSpawnerComponent>();
            var spawner = state.EntityManager.GetComponentData<MobSpawnerComponent>(mobSpawnerEntity);

            foreach (var element in buffer)
            {
                if (!_mobPrefabs.TryGetValue(element.MobID, out var prefab) || prefab.PrefabEntity.Equals(Entity.Null))
                {
                    Debug.LogError($"[MobSpawnSystem] Cannot found mob prefab entity id {element.MobID}!");
                    continue;
                }
                ref var data = ref prefab.AssetRef.Value;
                var mobEntity = state.EntityManager.Instantiate(prefab.PrefabEntity);

                var spawnLocation = element.Type == MobType.Freindly ? spawner.FriendlySpawnLocation : spawner.EnemySpawnLocation;
                state.EntityManager.AddComponentData(mobEntity, LocalTransform.FromPosition(spawnLocation));
                state.EntityManager.AddComponentData(mobEntity, new GameEntityComponent
                {
                    Entity = mobEntity,
                    ID = data.ID,
                    HP = data.HP,
                    EntityType = GameEntityType.Mob
                });
                state.EntityManager.AddComponentData(mobEntity, new MobComponent
                {
                    Entity = mobEntity,
                    Type = element.Type,
                    State = MobState.Move
                });
                state.EntityManager.AddComponentData(mobEntity, new GameEntityDataComponent(prefab.AssetRef));
                state.EntityManager.AddComponentData(mobEntity, new AnimatorComponent { AnimatorRef = prefab.Animator });
                state.EntityManager.AddComponentData(mobEntity, new SkillComponent());
                state.EntityManager.AddComponentData(mobEntity, new MobMoveStateComponent
                {
                    Direction = element.Type == MobType.Freindly ? 1 : -1,
                    Speed = data.Speed
                });


                var skillBuffer = state.EntityManager.AddBuffer<SkillBufferElement>(mobEntity);
                foreach (var item in SystemAPI.GetBuffer<SkillBufferElement>(prefab.PrefabEntity))
                {
                    skillBuffer.Add(new SkillBufferElement
                    {
                        Data = item.Data,
                        Cooldown = item.Data.Value.Cooldown
                    });
                }

                Debug.Log($"[MobSpawnSystem] Create new mob entity [{mobEntity.Index}]!");
            }

            // Clear buffer
            ecb.SetBuffer<MobSpawnBufferElement>(mobSpawnerEntity);
        }
    }
}

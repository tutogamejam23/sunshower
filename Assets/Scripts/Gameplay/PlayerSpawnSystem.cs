using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using UnityEngine;

namespace Sunshower
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct PlayerSpawnSystem : ISystem, ISystemStartStop
    {
        private NativeHashMap<int, PlayerPrefabBufferElement> _playerPrefabs;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpawnerComponent>();
        }

        public void OnStartRunning(ref SystemState state)
        {
            _playerPrefabs = new NativeHashMap<int, PlayerPrefabBufferElement>(64, Allocator.Persistent);

            var prefabBuffer = SystemAPI.GetSingletonBuffer<PlayerPrefabBufferElement>(isReadOnly: true);
            Debug.Assert(prefabBuffer.IsCreated);

            foreach (var prefab in prefabBuffer)
            {
                _playerPrefabs.Add(prefab.ID, prefab);
            }
        }

        public void OnStopRunning(ref SystemState state)
        {
            _playerPrefabs.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            var buffer = SystemAPI.GetSingletonBuffer<PlayerSpawnBufferElement>();
            if (buffer.IsEmpty)
            {
                return;
            }

            var ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var playerSpawnerEntity = SystemAPI.GetSingletonEntity<PlayerSpawnerComponent>();
            var spawner = state.EntityManager.GetComponentData<PlayerSpawnerComponent>(playerSpawnerEntity);

            foreach (var element in buffer)
            {
                if (!_playerPrefabs.TryGetValue(element.PlayerID, out var prefab))
                {
                    Debug.LogError($"[PlayerSpawnSystem] Cannot find player prefab entity id {element.PlayerID}!");
                    continue;
                }
                ref var data = ref prefab.AssetRef.Value;
                var playerEntity = state.EntityManager.Instantiate(prefab.PrefabEntity);

                state.EntityManager.AddComponentData(playerEntity, new GameEntityComponent
                {
                    Entity = playerEntity,
                    ID = data.ID,
                    HP = data.HP,
                    EntityType = GameEntityType.Player
                });
                state.EntityManager.AddComponentData(playerEntity, new PlayerComponent { Entity = playerEntity });
                state.EntityManager.AddComponentData(playerEntity, LocalTransform.FromPosition(spawner.SpawnLocation));
                state.EntityManager.AddComponentData(playerEntity, new GameEntityDataComponent(prefab.AssetRef));
                state.EntityManager.AddComponentData(playerEntity, new AnimatorComponent { AnimatorRef = prefab.Animator });
                state.EntityManager.AddComponentData(playerEntity, new SkillComponent());

                var skillBuffer = state.EntityManager.AddBuffer<SkillBufferElement>(playerEntity);
                foreach (var item in SystemAPI.GetBuffer<SkillBufferElement>(prefab.PrefabEntity))
                {
                    skillBuffer.Add(new SkillBufferElement
                    {
                        Data = item.Data,
                        Cooldown = item.Data.Value.Cooldown
                    });
                }

                Debug.Log($"[MobSpawnSystem] Create new player entity [{playerEntity.Index}]!");
            }

            // Clear buffer
            ecb.SetBuffer<PlayerSpawnBufferElement>(playerSpawnerEntity);
        }
    }
}

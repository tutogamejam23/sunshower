using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Sunshower
{
    public partial struct PlayerSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<StageComponent>();
            state.RequireForUpdate<PlayerPrefabComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonRW<StageComponent>(out var stage) || stage.ValueRO.State != StageState.Running)
            {
                return;
            }

            state.Enabled = false;

            // SystemAPI.Query<>()를 쓰면 foreach 중에 엔티티를 추가하는 코드가 있기 때문에 정상적으로 실행이 불가능함.
            var query = SystemAPI.QueryBuilder().WithAll<PlayerPrefabComponent>().Build();
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var playerObject = state.EntityManager.GetComponentObject<PlayerPrefabComponent>(entity);

                // Create player GameObject
                var go = Object.Instantiate(playerObject.Prefab, playerObject.SpawnPosition, Quaternion.identity);
                state.EntityManager.RemoveComponent<PlayerPrefabComponent>(entity);

                // Create player entity
                var playerEntity = state.EntityManager.CreateEntity();

                state.EntityManager.AddComponentObject(playerEntity, new PlayerInstanceComponent { Instance = go });
                state.EntityManager.AddComponentObject(playerEntity, go.GetComponent<Transform>());
                state.EntityManager.AddComponentObject(playerEntity, go.GetComponent<Animator>());

                state.EntityManager.AddComponent<GameEntityComponent>(playerEntity);
                state.EntityManager.AddComponent<PlayerComponent>(playerEntity);
                state.EntityManager.AddComponent<SkillComponent>(playerEntity);
            }
        }
    }

    public partial struct MobSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<StageComponent>();
        }
    }
}
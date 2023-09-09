using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    public class MobSpawnerAuthoring : MonoBehaviour
    {
        public Transform FriendlySpawnLocation;
        public Transform EnemySpawnLocation;

        private class Baker : Baker<MobSpawnerAuthoring>
        {
            public override void Bake(MobSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MobSpawnerComponent
                {
                    FriendlySpawnLocation = new float3(authoring.FriendlySpawnLocation.position),
                    EnemySpawnLocation = new float3(authoring.EnemySpawnLocation.position)
                });
                AddBuffer<MobSpawnBufferElement>(entity);
            }
        }
    }

    public readonly partial struct MobSpawnerAspect : IAspect
    {
        public readonly Entity Entity;
        public readonly RefRO<MobSpawnerComponent> Spawner;
    }

    public struct MobSpawnerComponent : IComponentData
    {
        public float3 FriendlySpawnLocation;
        public float3 EnemySpawnLocation;
    }
}

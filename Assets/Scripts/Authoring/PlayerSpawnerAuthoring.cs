using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    public class PlayerSpawnerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerSpawnerAuthoring>
        {
            public override void Bake(PlayerSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerSpawnerComponent
                {
                    SpawnLocation = new float3(authoring.transform.position)
                });
                AddBuffer<PlayerSpawnBufferElement>(entity);
            }
        }
    }

    public struct PlayerSpawnerComponent : IComponentData
    {
        public float3 SpawnLocation;
    }
}

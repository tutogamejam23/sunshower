using Unity.Entities;

using UnityEngine;

namespace Sunshower
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public Vector2 PlayerSpawnPosition;

        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new PlayerPrefabComponent
                {
                    Prefab = authoring.PlayerPrefab,
                    SpawnPosition = authoring.PlayerSpawnPosition
                });
            }
        }
    }
}

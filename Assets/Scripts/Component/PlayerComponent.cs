using System;

using Unity.Entities;

using UnityEngine;

namespace Sunshower
{
    public struct PlayerComponent : IComponentData
    {
    }

    public class PlayerPrefabComponent : IComponentData
    {
        public GameObject Prefab;
        public Vector2 SpawnPosition;
    }

    public class PlayerInstanceComponent : IComponentData, IDisposable
    {
        public GameObject Instance;

        public void Dispose()
        {
            UnityEngine.Object.Destroy(Instance);
        }
    }
}

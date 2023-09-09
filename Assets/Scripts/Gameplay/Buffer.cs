using Unity.Entities;
using Unity.Entities.Content;

using UnityEngine;

using GameEntityDataAssetRef = Unity.Entities.BlobAssetReference<Sunshower.GameEntityData>;

namespace Sunshower
{
    [InternalBufferCapacity(4)]
    public struct PlayerSpawnBufferElement : IBufferElementData
    {
        public int PlayerID;
    }

    [InternalBufferCapacity(32)]
    public struct MobSpawnBufferElement : IBufferElementData
    {
        public int MobID;
        public MobType Type;
    }

    public interface IPrefabBufferElement<T> : IBufferElementData where T : unmanaged
    {
        public int ID { get; }
        public Entity PrefabEntity { get; }
        public BlobAssetReference<T> AssetRef { get; }
        public WeakObjectReference<Animator> Animator { get; }
    }

    [InternalBufferCapacity(64)]
    public readonly struct PlayerPrefabBufferElement : IPrefabBufferElement<GameEntityData>
    {
        public readonly int ID { get; }
        public readonly Entity PrefabEntity { get; }
        public readonly GameEntityDataAssetRef AssetRef { get; }
        public readonly WeakObjectReference<Animator> Animator { get; }

        public PlayerPrefabBufferElement(int id, Entity prefab, GameEntityDataAssetRef assetRef, WeakObjectReference<Animator> animator)
        {
            ID = id;
            PrefabEntity = prefab;
            AssetRef = assetRef;
            Animator = animator;
        }
    }

    [InternalBufferCapacity(64)]
    public readonly struct MobPrefabBufferElement : IPrefabBufferElement<GameEntityData>
    {
        public readonly int ID { get; }
        public readonly Entity PrefabEntity { get; }
        public readonly GameEntityDataAssetRef AssetRef { get; }
        public readonly WeakObjectReference<Animator> Animator { get; }

        public MobPrefabBufferElement(int id, Entity prefab, GameEntityDataAssetRef assetRef, WeakObjectReference<Animator> animator)
        {
            ID = id;
            PrefabEntity = prefab;
            AssetRef = assetRef;
            Animator = animator;
        }
    }

    public interface IDataBufferElement<T> : IBufferElementData where T : unmanaged
    {
        public BlobAssetReference<T> Data { get; }
    }

    [InternalBufferCapacity(32)]
    public readonly partial struct GameDataBufferElement<T> : IDataBufferElement<T> where T : unmanaged, IBlobData
    {
        public readonly BlobAssetReference<T> Data { get; }

        public GameDataBufferElement(BlobAssetReference<T> data)
        {
            Data = data;
        }

        public static explicit operator GameDataBufferElement<T>(in BlobAssetReference<T> assetRef)
        {
            return new GameDataBufferElement<T>(assetRef);
        }
    }
}

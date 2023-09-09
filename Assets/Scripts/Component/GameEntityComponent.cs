using Unity.Entities;
using Unity.Entities.Content;

using UnityEngine;

namespace Sunshower
{
    public struct GameEntityComponent : IComponentData
    {
        public Entity Entity;
        public int ID;
        public int HP;
        public GameEntityType EntityType;
    }

    public struct AnimatorComponent : IComponentData
    {
        public bool IsLoaded => AnimatorRef.LoadingStatus == ObjectLoadingStatus.Completed;
        public bool IsStartedLoad { get; private set; }
        public WeakObjectReference<Animator> AnimatorRef { get; set; }

        public Animator Animator => AnimatorRef.Result;

        public void LoadAsync()
        {
            if (IsLoaded)
            {
                return;
            }
            AnimatorRef.LoadAsync();
            IsStartedLoad = true;
        }
    }
}

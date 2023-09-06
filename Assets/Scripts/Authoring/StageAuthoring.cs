using Unity.Entities;

using UnityEngine;

namespace Sunshower
{
    public class StageAuthoring : MonoBehaviour
    {
        [Header("Stage")]
        public float BeginDelay;

        private class Baker : Baker<StageAuthoring>
        {
            public override void Bake(StageAuthoring authoring)
            {
                var stageEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(stageEntity, new StageComponent
                {
                    State = StageState.Begin,
                    BeginDelay = authoring.BeginDelay,
                });
            }
        }
    }
}

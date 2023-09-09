using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sunshower
{

    public partial struct MobSystem : ISystem, ISystemStartStop
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MobComponent>();
        }

        public void OnStartRunning(ref SystemState state)
        {
            foreach (var mob in SystemAPI.Query<MobAspect>())
            {
                mob.Animator.ValueRO.LoadAsync();
            }
        }

        public void OnStopRunning(ref SystemState state)
        {
            foreach (var mob in SystemAPI.Query<MobAspect>())
            {
                mob.Animator.ValueRO.AnimatorRef.Release();
            }
        }

        public void OnUpdate(ref SystemState state)
        {
        }
    }

    public partial struct MobMoveStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MobMoveStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var deltaTime = SystemAPI.Time.DeltaTime;

            var entityQuery = SystemAPI.QueryBuilder().WithAny<PlayerAspect, MobAspect>().Build();
            var entities = entityQuery.ToEntityArray(Allocator.Temp);

            foreach (var (mob, moveState) in SystemAPI.Query<MobAspect, MobMoveStateComponent>())
            {
                var skills = SystemAPI.GetBuffer<SkillBufferElement>(mob.Entity);
                SkillBufferElement nextSkill = default;
                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);
                    if (nextSkill.Data != BlobAssetReference<SkillData>.Null)
                    {
                        if (skill.Cooldown > 0f || skill.Data.Value.Range < nextSkill.Data.Value.Range)
                        {
                            continue;
                        }
                    }
                    else if (skill.Cooldown > 0f)
                    {
                        continue;
                    }
                    nextSkill = skill; // 데이터 상 뒤에 있는 스킬 선택
                }

                if (nextSkill.Data != BlobAssetReference<SkillData>.Null)
                {
                    var mobTransform = SystemAPI.GetComponentRO<LocalTransform>(mob.Entity);
                    var mobPosition = mobTransform.ValueRO.Position;
                    var range = nextSkill.Data.Value.Range;
                    foreach (var targetEntity in entities)
                    {
                        var transform = SystemAPI.GetComponentRO<LocalTransform>(targetEntity);
                        var targetPosition = transform.ValueRO.Position;
                        var distance = math.distance(mobTransform.ValueRO.Position, transform.ValueRO.Position);
                        var predicate = (targetPosition.x - mobPosition.x) * moveState.Direction;
                        if (predicate < 0f || predicate > range)
                        {
                            continue;
                        }
                        ecb.AddComponent(mob.Entity, new MobAttackStateComponent
                        {
                            SkillID = nextSkill.Data.Value.ID,
                            Delay = nextSkill.Data.Value.Delay,
                            StartTime = (float)SystemAPI.Time.ElapsedTime,
                        });
                        ecb.RemoveComponent<MobMoveStateComponent>(mob.Entity);

                        return;
                    }
                }

                mob.Transform.ValueRW.Position += new float3(moveState.Direction * moveState.Speed * deltaTime, 0, 0);
            }
        }
    }

    public partial struct MobAttackStateSystem : ISystem
    {

    }
}

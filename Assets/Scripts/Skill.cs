using System;
using System.Buffers;

using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    public class Skill
    {
        public IGameEntity Owner { get; }
        public SkillData Data { get; }
        public float Cooldown { get => cooldown; set => cooldown = math.max(value, 0f); }
        public float DelayTime { get => delayTime; set => delayTime = math.max(value, 0f); }

        private float cooldown;
        private float delayTime;

        public Skill(IGameEntity owner, SkillData data)
        {
            Owner = owner;
            Data = data;
        }

        public bool Use()
        {
            if (Data.Cost > 0)
            {
                if (Owner is not ICostEntity costEntity)
                {
                    Debug.LogError($"Entity ID {Owner.ID}는 ICostEntity를 구현하지 않았습니다!");
                    return false;
                }
                if (costEntity.Cost < Data.Cost)
                {
                    return false;
                }
            }

            if (Cooldown > 0f || DelayTime > 0f)
            {
                return false;
            }

            if (Data.Damage > 0 && Data.Range > 0f)
            {
                var nearestEnties = ArrayPool<IGameEntity>.Shared.Rent(30);
                try
                {
                    var nearestCount = GetNearestEntity(Owner, Data.Range, ref nearestEnties);
                    if (nearestCount == 0)
                    {
                        return false;
                    }
                    IGameEntity targetEntity = null;
                    if (Owner is Player player)
                    {
                        for (int i = 0; i < nearestCount; i++)
                        {
                            IGameEntity entity = nearestEnties[i];
                            if (entity is Mob mob && mob.MobType == MobType.Enemy)
                            {
                                targetEntity = mob;
                                break;
                            }
                        }
                    }
                    else if (Owner is Mob mob)
                    {
                        for (int i = 0; i < nearestCount; i++)
                        {
                            IGameEntity entity = nearestEnties[i];
                            if (entity is Player playerTarget)
                            {
                                targetEntity = playerTarget;
                                break;
                            }
                            else if (entity is Mob mobTarget && mobTarget.MobType != mob.MobType)
                            {
                                targetEntity = mobTarget;
                                break;
                            }
                        }
                    }

                    if (targetEntity is null)
                    {
                        return false;
                    }
                    targetEntity.HP -= Data.Damage;
                }
                finally
                {
                    ArrayPool<IGameEntity>.Shared.Return(nearestEnties);
                }
            }

            Cooldown = Data.Cooldown;
            DelayTime = Data.Delay;
            // Debug.Log($"{Owner.ID}가 {Data.Name} 스킬을 사용했습니다. Cooldown: {Cooldown}, Delay: {DelayTime}");
            return true;
        }


        private struct TargetEntity
        {
            public static readonly TargetEntity Null = new() { ID = -1, Position = float3.zero };

            public int ID;
            public float3 Position;

            public override bool Equals(object obj)
                => obj is TargetEntity entity
                && Equals(entity);
            public bool Equals(TargetEntity other)
                => ID == other.ID
                && Position.Equals(other.Position);
            public override int GetHashCode() => HashCode.Combine(ID, Position);
            public static bool operator ==(TargetEntity left, TargetEntity right) => left.Equals(right);
            public static bool operator !=(TargetEntity left, TargetEntity right) => !(left == right);
        }

        public static int GetNearestEntity(IGameEntity target, float range, ref IGameEntity[] nearestEntity)
        {
            // 몹이 뒤에 있는 몹을 잡는 경우가 있나?
            var direction = target.Direction;
            var hits = ArrayPool<RaycastHit2D>.Shared.Rent(30);
            try
            {
                var hitCount = Physics2D.RaycastNonAlloc(target.Transform.position, new Vector2(direction.x, 0f), hits, range, 1 << 3); // Entity Layer
                if (hitCount == 0)
                {
                    return 0;
                }

                var entityCount = 0;
                for (int i = 0; i < hitCount; i++)
                {
                    var hit = hits[i];
                    if (hit.transform != target.Transform && hit.collider.TryGetComponent(out IGameEntity entity))
                    {
                        nearestEntity[entityCount++] = entity;
                    }
                }

                return entityCount;
            }
            finally
            {
                ArrayPool<RaycastHit2D>.Shared.Return(hits);
            }
        }
    }
}

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.Mathematics;

using UnityEngine;

namespace Sunshower
{
    [AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SkillAttribute : Attribute
    {
        public int ID { get; }

        public SkillAttribute(int id)
        {
            ID = id;
        }
    }

    public class Skill
    {
        public IGameEntity Owner { get; }
        public SkillManager Manager { get; set; }
        public SkillData Info { get; }
        public float Cooldown { get; set; }

        public Skill(SkillManager manager, SkillData info)
        {
            Owner = manager.Owner;
            Manager = manager;
            Info = info;
        }

        public bool CanUse()
        {
            if (Stage.Instance.CurrentState != Stage.Instance.RunningState)
            {
                return false;
            }
            if (Owner.HP == 0)
            {
                return false;
            }
            if (Manager.Delay > 0f)
            {
                return false;
            }
            if (Cooldown > 0f)
            {
                return false;
            }
            if (Owner is ICostEntity costEntity && costEntity.Cost < Info.Cost)
            {
                return false;
            }


            return true;
        }

        public bool Execute()
        {
            if (!CanUse())
            {
                return false;
            }

            using ArrayPool<IGameEntity> targetsPool = new(30);
            Span<IGameEntity> targets = targetsPool.Span;

            foreach (var command in Info.Commands)
            {
                bool ready = command switch
                {
                    // ready
                    SkillCommand.GetTargetsInMobRange getTargetsInMobRange => GetTargetsInMobRange(getTargetsInMobRange, ref targets),
                    _ => true
                };
                if (!ready)
                {
                    return false;
                }

                switch (command)
                {
                    case SkillCommand.AttackTarget attackTarget:
                        AttackTarget(attackTarget, in targets);
                        break;

                    case SkillCommand.ShotProjectile shotProjectile:
                        ShotProjectile(shotProjectile);
                        break;

                    case SkillCommand.InstallSkillToPosition installSkillToPosition:
                        InstallSkillToPosition(installSkillToPosition, Manager.UsePosition);
                        break;

                    case SkillCommand.SpawnRandomMobs spawnRandomMobs:
                        SpawnRandomMobs(spawnRandomMobs);
                        break;

                    case SkillCommand.BuffToAll buffToAll:
                        BuffToAll(buffToAll);
                        break;

                    case SkillCommand.PlayAreaEffect playAreaEffect:
                        PlayAreaEffect(playAreaEffect);
                        break;

                    case SkillCommand.PlayEffectToTarget playEffectToTarget:
                        PlayEffectToTarget(playEffectToTarget, in targets);
                        break;

                    default:
                        break;
                }
            }

            // Set animation
            var animation = Owner.Animation;
            var entry = animation.state.SetAnimation(0, Info.Animation, false);
            Manager.Delay = Info.Delay > 0f ? Info.Delay : entry.AnimationEnd;

            SoundManager.instance.PlaySFXAtPosition(Info.SFX, Owner.Transform.position);

            Manager.UsePosition = Vector3.zero;
            Cooldown = Info.Cooldown;
            if (Owner is ICostEntity costEntity)
            {
                costEntity.Cost -= Info.Cost;
            }

#if UNITY_EDITOR
            if (Owner is Player player && player.LogEnabled)
            {
                Debug.Log($"플레이어가 {Info.Name} 스킬을 사용했습니다.");
            }
            else if (Owner is Mob mob && mob.LogEnabled)
            {
                Debug.Log($"몹({mob.name})이 {Info.Name} 스킬을 사용했습니다.");
            }
#endif
            return true;
        }

        private void PlayEffectToTarget(SkillCommand.PlayEffectToTarget playEffectToTarget, in Span<IGameEntity> targets)
        {
            foreach (var target in targets)
            {
                Stage.Instance.EffectManager.Play(playEffectToTarget.Effect, target.Transform.position);
            }
        }

        private void InstallSkillToPosition(SkillCommand.InstallSkillToPosition installSkillToPosition, Vector3 usePosition)
        {
            if (usePosition == Vector3.zero)
            {
                return;
            }

            Stage.Instance.ProjectileManager.Shot(
                installSkillToPosition.Prefab, usePosition, Vector3.zero, 0,
                installSkillToPosition.HitTarget, installSkillToPosition.HitCount, installSkillToPosition.OnHit);
        }

        private bool GetTargetsInMobRange(SkillCommand.GetTargetsInMobRange getTargetsInMobRange, ref Span<IGameEntity> targets)
        {
            if (Owner is not Mob mob) // GetTargetsInRange는 Mob 전용
            {
                Debug.LogError("Owner is not Mob!");
                return false;
            }
            Debug.Assert(getTargetsInMobRange.TargetCount > 0);

            var isCharming = Manager.IsCharming > 0;
            var mobData = mob.Data as MobData;
            var direction = !isCharming ? mob.Direction : mob.Direction * -1f;
            var targetSide = !isCharming ? getTargetsInMobRange.Target : getTargetsInMobRange.Target switch
            {
                EntitySideType.Friendly => EntitySideType.Enemy,
                EntitySideType.Enemy => EntitySideType.Friendly,
                _ => throw new NotImplementedException()
            };

            var count = GetNearestEntities(Owner, direction, mobData.Range, ref targets, filter: targetSide);
            if (getTargetsInMobRange.TargetCount < count) // Span 길이를 원하는 만큼 잘라서 반환
            {
                count = getTargetsInMobRange.TargetCount;
            }
            targets = targets[..count];

            return count > 0; // 범위 내에 타겟이 있으면 성공
        }

        private void AttackTarget(SkillCommand.AttackTarget attackTarget, in Span<IGameEntity> targets)
        {
            foreach (var target in targets)
            {
                target.HP -= attackTarget.Damage;
            }
        }

        private void SpawnRandomMobs(SkillCommand.SpawnRandomMobs spawnRandomMobs)
        {
            // 리스트가 2개 이상이면 랜덤 선택
            var randomIdx = UnityEngine.Random.Range(0, spawnRandomMobs.Queue.Count);
            Stage.Instance.StartCoroutine(SpawnMobs(spawnRandomMobs.Queue[randomIdx], spawnRandomMobs.SpawnDelay, Owner.EntitySide));
        }

        private IEnumerator SpawnMobs(List<int> mobs, float spawnDelay, EntitySideType entitySide)
        {
            var wait = new WaitForSeconds(spawnDelay);
            foreach (var mobID in mobs)
            {
                Stage.Instance.MobSpawner.Spawn(mobID, entitySide);
                yield return wait;
            }
        }

        private void ShotProjectile(SkillCommand.ShotProjectile shotProjectile)
        {
            Stage.Instance.StartCoroutine(ShotProjectileWithDelay(shotProjectile));
        }

        private IEnumerator ShotProjectileWithDelay(SkillCommand.ShotProjectile info)
        {
            var wait = new WaitForSeconds(info.ShotDelay);
            for (int i = 0; i < info.Count; i++)
            {
                var position = Owner.Transform.position;
                position.y = UnityEngine.Random.Range(position.y - 1f, position.y + 1f);
                position.y += 2f;

                Stage.Instance.ProjectileManager.Shot(
                    info.Prefab, position, Owner.Direction, info.Speed, info.HitTarget, info.HitCount, info.OnHit);

                yield return wait;
            }
        }

        private void PlayAreaEffect(SkillCommand.PlayAreaEffect playAreaEffect)
        {
            Stage.Instance.EffectManager.Play(playAreaEffect.Effect, Vector3.zero);
            SoundManager.instance.PlaySFX(playAreaEffect.SFX);
        }

        private void BuffToAll(SkillCommand.BuffToAll buffToAll)
        {
            switch (buffToAll.Target)
            {
                case EntitySideType.Friendly:
                    // Buff 타입이 struct라서 그냥 인자로 넘겨도 알아서 깊은복사가 됨
                    Stage.Instance.ActivePlayer.SkillManager.AddBuff(buffToAll.Buff);
                    foreach (var friendlyMob in Stage.Instance.MobSpawner.ActiveFriendlyMobs)
                    {
                        friendlyMob.SkillManager.AddBuff(buffToAll.Buff);
                    }
                    break;

                case EntitySideType.Enemy:
                    foreach (var enemyMob in Stage.Instance.MobSpawner.ActiveEnemyMobs)
                    {
                        enemyMob.SkillManager.AddBuff(buffToAll.Buff);
                    }
                    break;

                default:
                    break;
            }
        }

        public static int GetNearestEntities(IGameEntity target, Vector3 direction, float range, ref Span<IGameEntity> nearestEntities, EntitySideType filter = EntitySideType.None)
        {
            using var hits = new ArrayPool<RaycastHit2D>(nearestEntities.Length);
            var hitCount = Physics2D.RaycastNonAlloc(target.Transform.position, new Vector2(direction.x, 0f), hits.Array, range, 1 << 3); // Entity Layer
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
                    if (entity.HP == 0)
                    {
                        continue;
                    }
                    if (filter != EntitySideType.None && entity.EntitySide != filter)
                    {
                        continue;
                    }
                    nearestEntities[entityCount++] = entity;
                }
            }

            return entityCount;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Sunshower
{
    public class Buff
    {
        public float Duration { get; set; }
        public float DotInterval { get; set; }
        public int DotDamage { get; set; }
        public int DotHeal { get; set; }
        public float SpeedRate { get; set; }
        public bool Charm { get; set; }

        [YamlIgnore]
        public bool IsNull { get; private set; }

        [YamlIgnore]
        public IGameEntity Owner { get; set; }

        [YamlIgnore]
        public SkillManager Manager { get; set; }

        /// <summary>
        /// 버프 시간
        /// </summary>
        [YamlIgnore]
        public float Time { get; set; }

        private float _dotTime;

        public static Buff Null => new() { IsNull = true };

        public void OnUpdate(float deltaTime)
        {
            _dotTime += deltaTime;
            if (_dotTime >= DotInterval)
            {
                Owner.HP -= DotDamage;
                Owner.HP += DotHeal;
                _dotTime = 0f;
            }
        }

        public void OnAdd()
        {
            if (Charm && Owner is Mob mob && !mob.MobData.ResistCharming)
            {
                ++Manager.IsCharming;
            }
        }

        public void OnRemove()
        {
            if (Charm && Owner is Mob mob && !mob.MobData.ResistCharming)
            {
                --Manager.IsCharming;
            }
        }
    }

    public class SkillManager : MonoBehaviour
    {
        public IGameEntity Owner { get; private set; }
        public List<Skill> Skills { get; } = new();
        public IReadOnlyCollection<Buff> Buffs => _buffs;
        public float SpeedRateBuffAverage
        {
            get
            {
                var speedRate = 0f;
                var count = 0;
                foreach (var buff in _buffs)
                {
                    if (buff.SpeedRate > 0f)
                    {
                        speedRate += buff.SpeedRate;
                        ++count;
                    }
                }
                return count > 0 ? speedRate / count : 1f;
            }
        }

        // 스킬에서 사용하는 필드
        public Vector3 UsePosition { get; set; }

        public float Delay { get; set; }

        // 여러 버프에서 매혹을 걸 경우 이를 카운팅해서 모든 버프의 매혹이 풀릴 때까지 매혹 상태를 유지
        public int IsCharming { get; set; }

        private readonly Dictionary<int, int> _indexMap = new();
        private readonly LinkedList<Buff> _buffs = new();

        public void Register(IGameEntity owner, IEnumerable<int> skills)
        {
            Owner = owner;
            foreach (var skillID in skills)
            {
                var data = Stage.Instance.DataTable.SkillTable[skillID];
                var skill = new Skill(this, data);
                Skills.Add(skill);
                _indexMap.Add(skillID, Skills.Count - 1);
            }
        }

        public Skill GetSkill(int skillID)
        {
            if (!_indexMap.TryGetValue(skillID, out var index))
            {
                return null;
            }
            return Skills[index];
        }

        public void AddBuff(Buff buff)
        {
            var newBuff = new Buff
            {
                Owner = Owner,
                Manager = this,
                Duration = buff.Duration,
                DotInterval = buff.DotInterval,
                DotDamage = buff.DotDamage,
                DotHeal = buff.DotHeal,
                SpeedRate = buff.SpeedRate,
                Charm = buff.Charm
            };
            _buffs.AddLast(newBuff);
            newBuff.OnAdd();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            foreach (var skill in Skills)
            {
                skill.Cooldown = math.max(skill.Cooldown - deltaTime, 0f);
            }
            Delay = math.max(Delay - deltaTime, 0f);

            var node = _buffs.Last;
            while (node != null) // OnUpdate 이후 버프가 끝나면 리스트에서 제거되기 때문에 리스트가 변경됨
            {
                var buff = node.Value;
                var current = node;
                node = node.Previous;
                buff.OnUpdate(deltaTime);

                buff.Time += deltaTime;
                if (buff.Time >= buff.Duration)
                {
                    buff.OnRemove();
                    _buffs.Remove(current);
                }
            }

            if (IsCharming > 0)
            {
                if (Owner is Mob mob && mob.Animation.SkeletonDataAsset.name != mob.CharmingSkin)
                {
                    mob.Animation.Skeleton.SetSkin(mob.CharmingSkin);
                    mob.Animation.Skeleton.ScaleX = -mob.Animation.Skeleton.ScaleX;
                }
            }
            else
            {
                if (Owner is Mob mob && mob.Animation.SkeletonDataAsset.name != mob.DefaultSkin)
                {
                    mob.Animation.Skeleton.SetSkin(mob.DefaultSkin);
                    mob.Animation.Skeleton.ScaleX = math.abs(mob.Animation.Skeleton.ScaleX);
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class SkillManager : MonoBehaviour
    {
        public Dictionary<int, Skill> SkillMap { get; } = new Dictionary<int, Skill>();

        public void Initialize(IGameEntity owner, IEnumerable<SkillData> skills)
        {
            foreach (var data in skills)
            {
                var skill = new Skill(owner, data);
                SkillMap.Add(data.ID, skill);
            }
        }

        private void Update()
        {
            foreach (var skill in SkillMap.Values)
            {
                if (skill.Cooldown > 0f)
                {
                    skill.Cooldown -= Time.deltaTime;
                }

                if (skill.DelayTime > 0f)
                {
                    skill.DelayTime -= Time.deltaTime;
                }
            }
        }
    }
}

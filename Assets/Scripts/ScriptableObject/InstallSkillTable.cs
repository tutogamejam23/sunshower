using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class InstallSkillTable : MonoBehaviour
    {
        [SerializeField] private List<InstallSkill> _installSkills = new();

        public Dictionary<string, InstallSkill> InstallSkills { get; private set; }

        public void CreateTable()
        {
            InstallSkills = new Dictionary<string, InstallSkill>();
            foreach (var installSkill in _installSkills)
            {
                if (installSkill == null)
                {
                    continue;
                }
                InstallSkills.TryAdd(installSkill.name, installSkill);
            }
        }
    }
}

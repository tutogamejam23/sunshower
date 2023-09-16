using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class AxeAttack : MonoBehaviour ,ISkill<Mob>
    {
        float cooldown = 0;
        int cost = 0;

        public float GetCooldown()
        {
            return cooldown;
        }

        public int GetCost()
        {
            return cost;
        }

        public void Use(Mob owener)
        {
            Debug.Log("도끼 공격 시전");
        }
    }
}

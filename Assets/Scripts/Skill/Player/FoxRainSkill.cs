using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class FoxRainSkill : MonoBehaviour ,ISkill<Player>
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

        public void Use(Player owener)
        {
            Debug.Log("FoxRainSkill 실행");
        }
    }
}

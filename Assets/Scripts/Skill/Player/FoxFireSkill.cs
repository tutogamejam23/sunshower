using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class FoxFireSkill : MonoBehaviour, ISkill<Player>
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
            Debug.Log("FoxFireSkill 실행");
        }
    }
}

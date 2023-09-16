using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class GunShot : MonoBehaviour
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
            Debug.Log("GunShot");
        }
    }
}

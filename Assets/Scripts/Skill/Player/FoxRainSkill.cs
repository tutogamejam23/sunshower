using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class FoxRainSkill : MonoBehaviour ,ISkill<Player>
    {
        public void Use(Player owener)
        {
            Debug.Log("FoxRainSkill 실행");
        }
    }
}

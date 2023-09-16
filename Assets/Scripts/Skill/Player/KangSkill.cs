using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class KangSkill : MonoBehaviour, ISkill<Player>
    {
        public void Use(Player owener)
        {
            Debug.Log("KangSkill 실행");
        }
    }
}

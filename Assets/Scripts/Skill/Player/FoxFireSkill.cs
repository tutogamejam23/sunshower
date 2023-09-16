using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class FoxFireSkill : MonoBehaviour, ISkill<Player>
    {
        [SerializeField] float speed = 3;

        public void Use(Player owener)
        {
            Debug.Log("FoxFireSkill 실행");
        }
    }
}

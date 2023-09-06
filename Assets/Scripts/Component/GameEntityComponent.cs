using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sunshower
{
    public struct GameEntityComponent : IComponentData
    {
        public int HP;
    }
}

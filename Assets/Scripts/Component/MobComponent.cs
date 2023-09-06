using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sunshower
{
    /// <summary>
    /// 아군 / 적군
    /// </summary>
    public enum MobType
    {
        Freindly = 1,
        Enemy = 2
    }

    public struct MobComponent : IComponentData
    {
        public MobType Type;
    }
}

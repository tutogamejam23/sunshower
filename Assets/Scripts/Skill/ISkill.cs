using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public interface ISkill<T>
    {
        void Use(T owener);
    }
}

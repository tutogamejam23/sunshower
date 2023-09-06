using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sunshower
{
    public class SkillAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SkillAuthoring>
        {
            public override void Bake(SkillAuthoring authoring)
            {
            }
        }
    }
}

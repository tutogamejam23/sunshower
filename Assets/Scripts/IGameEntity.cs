using Spine.Unity;
using UnityEngine;

namespace Sunshower
{
    public enum EntitySideType
    {
        None, Friendly, Enemy
    }

    public interface IGameEntity
    {
        public Transform Transform { get; }
        public SkeletonAnimation Animation { get; }
        public Vector3 Direction { get; }
        public EntitySideType EntitySide { get; }
        public int ID { get; }
        public int HP { get; set; }
    }

    public interface ICostEntity
    {
        public int Cost { get; set; }
    }

    public interface ISkillEntity
    {
        public SkillManager SkillManager { get; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public interface IDataTableItem
    {
        int ID { get; }
    }

    public class GameEntityData : IDataTableItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string PrefabPath { get; set; }
        public int HP { get; set; }
        public float Speed { get; set; }
        public int[] Skills { get; set; }
    }

    public class PlayerData : GameEntityData
    {
        public string IdleAnimation { get; set; }
        public string DeadAnimation { get; set; }
        public string DeadSFX { get; set; }
        public int MaxCost { get; set; }
        public float CostUpTime { get; set; }
    }

    public class MobData : GameEntityData
    {
        public string MoveAnimation { get; set; }
        public string DeadAnimation { get; set; }
        public string MoveSFX { get; set; }
        public string DeadSFX { get; set; }
        public float Range { get; set; }
        public bool ResistCharming { get; set; }
    }

    public class SkillData : IDataTableItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Animation { get; set; }
        public string SFX { get; set; }
        public int Cost { get; set; }
        public float Delay { get; set; }
        public float Cooldown { get; set; }
        public SkillCommandBase[] Commands { get; set; }
    }

    public class SkillCommandBase
    {
        public string Command { get; set; }
    }

    public static class SkillCommand
    {
        public class GetTargetsInMobRange : SkillCommandBase
        {
            public EntitySideType Target { get; set; }
            public int TargetCount { get; set; }
        }

        public class AttackTarget : SkillCommandBase
        {
            public int Damage { get; set; }
        }

        public class SpawnRandomMobs : SkillCommandBase
        {
            public List<List<int>> Queue { get; set; }
            public float SpawnDelay { get; set; }
        }

        public class ShotProjectile : SkillCommandBase
        {
            public string Prefab { get; set; }
            public ShotType Shot { get; set; }
            public float Speed { get; set; }
            public int Count { get; set; }
            public float ShotDelay { get; set; }
            public EntitySideType HitTarget { get; set; }
            public int HitCount { get; set; }
            public HitSideEffect OnHit { get; set; }
        }

        public class InstallSkillToPosition : SkillCommandBase
        {
            public string Prefab { get; set; }
            public float Duration { get; set; }
            public EntitySideType HitTarget { get; set; }
            public int HitCount { get; set; }
            public HitSideEffect OnHit { get; set; }
        }

        public class PlayAreaEffect : SkillCommandBase
        {
            public string Effect { get; set; }
            public string SFX { get; set; }
            public float Duration { get; set; }
        }

        public class PlayEffectToTarget : SkillCommandBase
        {
            public string Effect { get; set; }
        }

        public class BuffToAll : SkillCommandBase
        {
            public EntitySideType Target { get; set; }
            public Buff Buff { get; set; } = Buff.Null;
        }

        public class HitSideEffect
        {
            public int Damage { get; set; }
            public Buff Buff { get; set; } = Buff.Null;
        }
    }

    public class StageData : IDataTableItem
    {
        public int ID => StageNumber;
        public int StageNumber { get; set; }
        public int PlayerID { get; set; }
        public float SpawnDelay { get; set; }
        public int[] Spawner { get; set; }
    }

}

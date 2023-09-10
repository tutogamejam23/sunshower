using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Sunshower
{
    [Flags]
    public enum GameEntityType
    {
        None = 0,
        Player = 1,
        Mob = 2,
        Projectile = 3
    }

    public interface IDataTableItem
    {
        int ID { get; }
    }

    public class GameEntityData : IDataTableItem
    {
        public GameEntityType EntityType { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string PrefabPath { get; set; }
        public int HP { get; set; }
        public float Speed { get; set; }
        public int[] Skills { get; set; }
    }

    public class SkillData : IDataTableItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Animation { get; set; }
        public int Cost { get; set; }
        public float Delay { get; set; }
        public float Cooldown { get; set; }
        public int Damage { get; set; }
        public float Range { get; set; }
        public float Duration { get; set; }
        public int HealAmount { get; set; }
        public float SpeedDecreaseRate { get; set; }
        public float SpeedDecreaseTime { get; set; }
        public int[] SpawnMobIDs { get; set; }
        public int[] SpawnMobCounts { get; set; }
    }

    public class StageData : IDataTableItem
    {
        public int ID => StageNumber;
        public int StageNumber { get; set; }
        public int PlayerID { get; set; }
        public float SpawnDelay { get; set; }
        public int[] Spawner { get; set; }
    }

    public class DataTableManager : MonoBehaviour
    {
        public static readonly string PlayerTablePath = "DataTable/PlayerTable";
        public static readonly string MobTablePath = "DataTable/MobTable";
        public static readonly string SkillTablePath = "DataTable/SkillTable";
        public static readonly string StageTablePath = "DataTable/StageTable";

        private static readonly IDeserializer DataTableDeserializer =
            new DeserializerBuilder()
            .Build();

        public IReadOnlyDictionary<int, GameEntityData> PlayerTable => _playerTable;
        public IReadOnlyDictionary<int, GameEntityData> MobTable => _mobTable;
        public IReadOnlyDictionary<int, SkillData> SkillTable => _skillTable;
        public IReadOnlyDictionary<int, StageData> StageTable => _stageTable;

        private Dictionary<int, GameEntityData> _playerTable;
        private Dictionary<int, GameEntityData> _mobTable;
        private Dictionary<int, SkillData> _skillTable;
        private Dictionary<int, StageData> _stageTable;

        private void Awake()
        {
            _playerTable = ImportDataTable<GameEntityData>(PlayerTablePath);
            _mobTable = ImportDataTable<GameEntityData>(MobTablePath);
            _skillTable = ImportDataTable<SkillData>(SkillTablePath);
            _stageTable = ImportDataTable<StageData>(StageTablePath);
        }

        private static Dictionary<int, T> ImportDataTable<T>(in string path) where T : IDataTableItem
        {
            var dataText = Resources.Load<TextAsset>(path);
            var table = DataTableDeserializer.Deserialize<T[]>(dataText.text);
            var map = new Dictionary<int, T>();
            foreach (var data in table)
            {
                map.Add(data.ID, data);
            }
            return map;
        }
    }
}

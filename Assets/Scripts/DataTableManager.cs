using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Sunshower
{
    public class DataTableManager : MonoBehaviour
    {
        public static readonly string PlayerTablePath = "DataTable/PlayerTable";
        public static readonly string MobTablePath = "DataTable/MobTable";
        public static readonly string SkillTablePath = "DataTable/SkillTable";
        public static readonly string StageTablePath = "DataTable/StageTable";

        public IReadOnlyDictionary<int, PlayerData> PlayerTable => _playerTable;
        public IReadOnlyDictionary<int, MobData> MobTable => _mobTable;
        public IReadOnlyDictionary<int, SkillData> SkillTable => _skillTable;
        public IReadOnlyDictionary<int, StageData> StageTable => _stageTable;

        private Dictionary<int, PlayerData> _playerTable;
        private Dictionary<int, MobData> _mobTable;
        private Dictionary<int, SkillData> _skillTable;
        private Dictionary<int, StageData> _stageTable;

        private void Awake()
        {
            _playerTable =
                ImportDataTable<PlayerData[]>(PlayerTablePath, PlayerDataTableDeserializer.Deserializer)
                .ToDictionary(data => data.ID);

            _mobTable =
                ImportDataTable<MobData[]>(MobTablePath, MobDataTableDeserializer.Deserializer)
                .ToDictionary(data => data.ID);

            _skillTable =
                ImportDataTable<SkillData[]>(SkillTablePath, SkillDataTableDeserializer.Deserializer)
                .ToDictionary(data => data.ID);

            _stageTable =
                ImportDataTable<StageData[]>(StageTablePath, StageDataTableDeserializer.Deserializer)
                .ToDictionary(data => data.ID);
        }

        private static T ImportDataTable<T>(in string path, IDeserializer deserializer)
        {
            var dataText = Resources.Load<TextAsset>(path);
            return deserializer.Deserialize<T>(dataText.text);
        }
    }
}

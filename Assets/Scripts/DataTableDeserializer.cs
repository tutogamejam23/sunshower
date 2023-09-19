using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Sunshower
{
    public static class PlayerDataTableDeserializer
    {
        public static IDeserializer Deserializer =>
            new DeserializerBuilder()
            .Build();
    }

    public static class MobDataTableDeserializer
    {
        public static IDeserializer Deserializer =>
            new DeserializerBuilder()
            .Build();
    }

    public static class SkillDataTableDeserializer
    {
        private static readonly Dictionary<string, Type> s_typeMap = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnInitialized()
        {
            var executes = typeof(SkillCommand).GetNestedTypes();
            foreach (var execute in executes)
            {
                if (execute.BaseType != typeof(SkillCommandBase))
                {
                    continue;
                }
                s_typeMap.TryAdd(execute.Name, execute);
            }
        }

        public static IDeserializer Deserializer =>
            new DeserializerBuilder()
            .WithTypeDiscriminatingNodeDeserializer(options =>
            {
                options.AddKeyValueTypeDiscriminator<SkillCommandBase>("Command", s_typeMap);
            })
            .Build();
    }

    public static class StageDataTableDeserializer
    {
        public static IDeserializer Deserializer =>
            new DeserializerBuilder()
            .Build();
    }
}

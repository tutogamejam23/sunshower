using System.Text.Json;
using System.Text.Json.Nodes;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Sunshower
{
    public class GameDataAuthoring : MonoBehaviour
    {
        private class Baker : Baker<GameDataAuthoring>
        {
            public override void Bake(GameDataAuthoring authoring)
            {
                var skillTable = LoadSkilTable();
            }

            private static BlobBuilder LoadSkilTable()
            {
                TextAsset skillTableJson = Resources.Load<TextAsset>("SkillTable");

                JsonDocumentOptions options = new()
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                };
                JsonNode document = JsonNode.Parse(skillTableJson.text, documentOptions: options);
                JsonArray jsonArray = document.AsArray();
                BlobBuilder builder = new(Allocator.Temp);
                ref SkillDataPool pool = ref builder.ConstructRoot<SkillDataPool>();
                BlobBuilderArray<SkillData> skills = builder.Allocate(ref pool.Skills, jsonArray.Count);

                for (int i = 0; i < jsonArray.Count; i++)
                {
                    JsonNode node = jsonArray[i];
                    ref SkillData skill = ref skills[i];
                    skill = new SkillData();

                    // TODO: json에 잘못된 타입 값이 들어왔을 때 예외처리 필요함
                    if (node["id"] is JsonNode id)
                    {
                        skill.ID = (int)id;
                    }
                    if (node["name"] is JsonNode name)
                    {
                        builder.AllocateString(ref skill.Name, (string)name);
                    }
                    if (node["description"] is JsonNode desc)
                    {
                        builder.AllocateString(ref skill.Description, (string)desc);
                    }
                    if (node["cost"] is JsonNode cost)
                    {
                        // skill.Cost = (int?)cost ?? throw new System.Text.Json.exce;
                    }
                    if (node["cooldown"] is JsonNode cooldown)
                    {
                        skill.CoolDown = (float?)cooldown ?? 0f;
                    }
                    if (node["damage"] is JsonNode damage)
                    {
                    }
                }

                return builder;
            }
        }
    }
}
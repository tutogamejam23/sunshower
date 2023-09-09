using System;
using System.Text;

using Unity.Entities;

namespace Sunshower
{
    internal static class BlobArrayExtensions
    {
        public static void Init<T>(this ref BlobArray<T> target, ref BlobBuilder builder, in T[] source) where T : unmanaged
        {
            var arr = builder.Allocate(ref target, source.Length);
            unsafe
            {
                var destSpan = new Span<T>(arr.GetUnsafePtr(), arr.Length);
                var sourceSpan = new ReadOnlySpan<T>(source);
                sourceSpan.CopyTo(destSpan);
            }
        }

        public static void InitUTF8String(this ref BlobArray<byte> target, ref BlobBuilder builder, in string source)
        {
            var destArr = builder.Allocate(ref target, Encoding.UTF8.GetByteCount(source));
            unsafe
            {
                var destSpan = new Span<byte>(destArr.GetUnsafePtr(), destArr.Length);
                Encoding.UTF8.GetBytes(source, destSpan);
            }
        }

        public static string GetUTF8String(this ref BlobArray<byte> target)
        {
            unsafe
            {
                var span = new ReadOnlySpan<byte>(target.GetUnsafePtr(), target.Length);
                return Encoding.UTF8.GetString(span);
            }
        }
    }

    public interface IBlobData
    {
    }

    [Flags]
    public enum GameEntityType
    {
        None = 0,
        Player = 1,
        Mob = 2,
        Projectile = 3
    }

    public struct GameEntityData : IBlobData
    {
        public GameEntityType EntityType;
        public int ID;
        public BlobArray<byte> Name;
        public BlobArray<byte> Prefab;
        public int HP;
        public float Speed;
        public BlobArray<int> Skills;
    }

    // TODO: Raw 클래스 안쓰고 파싱하는 법 없나? 중복 데이터가 너무 거슬린다...
    internal class GameEntityDataRaw
    {
        public int ID = 0;
        public string Name = string.Empty;
        public string Prefab = string.Empty;
        public int HP = 0;
        public float Speed = 0f;
        public int[] Skills = Array.Empty<int>();
    }

    public struct SkillData : IBlobData
    {
        public int ID;
        public BlobArray<byte> Name;
        public BlobArray<byte> Description;
        public BlobArray<byte> Animation;
        public int Cost;
        public float Delay;
        public float Cooldown;
        public int Damage;
        public float Range;
        public float Duration;
        public int HealAmount;
        public float SpeedDecreaseRate;
        public float SpeedDecreaseTime;
        public BlobArray<int> SpawnMobIDs;
        public BlobArray<int> SpawnMobCounts;
    }

    internal class SkillDataRaw
    {
        public int ID = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public string Animation = string.Empty;
        public int Cost = 0;
        public float Delay = 0f;
        public float Cooldown = 0f;
        public int Damage = 0;
        public float Range = 0f;
        public float Duration = 0;
        public int HealAmount = 0;
        public float SpeedDecreaseRate = 0;
        public float SpeedDecreaseTime = 0;
        public int[] SpawnMobIDs = Array.Empty<int>();
        public int[] SpawnMobCounts = Array.Empty<int>();
    }
}

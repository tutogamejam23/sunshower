using Unity.Entities;

namespace Sunshower
{
    public enum StageState
    {
        Begin = 0,
        Running = 1,
        End = 2
    }

    public struct StageComponent : IComponentData
    {
        public StageState State;
        public float BeginDelay;
    }
}
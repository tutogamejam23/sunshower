using Unity.Entities;

namespace Sunshower
{
    public partial struct StageSystem : ISystem
    {
        private bool _isBegin;
        private float _beginTimer;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<StageComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var stage = SystemAPI.GetSingletonRW<StageComponent>();
            var stageState = stage.ValueRW.State;
            if (stageState == StageState.Begin)
            {
                if (!_isBegin)
                {
                    _beginTimer = stage.ValueRO.BeginDelay;
                    _isBegin = true;
                }
                _beginTimer -= SystemAPI.Time.DeltaTime;

                if (_beginTimer < 0f)
                {
                    stage.ValueRW.State = StageState.Running;
                }
                return;
            }

            if (stageState == StageState.Running)
            {

            }
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
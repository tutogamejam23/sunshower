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

                    var playerSpawnBuffer = SystemAPI.GetSingletonBuffer<PlayerSpawnBufferElement>();
                    playerSpawnBuffer.Add(new PlayerSpawnBufferElement { PlayerID = 0 });

                    var mobSpawnBuffer = SystemAPI.GetSingletonBuffer<MobSpawnBufferElement>();
                    mobSpawnBuffer.Add(new MobSpawnBufferElement { MobID = 200, Type = MobType.Enemy });
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

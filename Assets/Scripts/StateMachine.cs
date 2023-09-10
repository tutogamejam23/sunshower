using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public interface IState<in T> where T : MonoBehaviour
    {
        /// <summary>
        /// State 재활용이 필요할 경우 내부 필드를 초기화합니다. 오브젝트 풀링을 사용하여 State가 계속 메모리에 존재하는 경우 사용합니다.
        /// </summary>
        void Initialize();
        void Enter(T owner);
        void Execute(T owner);
        void Exit(T owner);
    }

    public class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
    {
        public IState<T> CurrentState { get; private set; }
        public IState<T> PreviousState { get; private set; }

        public void ChangeState(IState<T> newState)
        {
            if (newState == CurrentState)
            {
                Debug.LogWarning("같은 State로 변경하려고 했습니다!");
                return;
            }

            CurrentState?.Exit(this as T);
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter(this as T);
        }

        protected virtual void Update()
        {
            CurrentState?.Execute(this as T);
        }

        protected virtual void OnDestroy()
        {
            CurrentState?.Exit(this as T);
        }
    }
}

using System;
using UnityEngine;

namespace StateMachineComponents
{
    public abstract class FiniteStateBehaviour : MonoBehaviour
    {
        public event Action<IState> OnEntityStateChanged;
        protected StateMachine stateMachine;
        public IState CurrentStateType => stateMachine.CurrentState;

        protected virtual void Awake()
        {
            stateMachine = new StateMachine();
            stateMachine.OnStateChanged += state => OnEntityStateChanged?.Invoke(state);
        }

        protected virtual void Update() => stateMachine.Tick();
        protected virtual void FixedUpdate() => stateMachine.FixedTick();
    }
}
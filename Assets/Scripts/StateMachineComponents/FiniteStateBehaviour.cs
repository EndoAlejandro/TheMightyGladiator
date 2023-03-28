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
            References();
            stateMachine = new StateMachine();
            stateMachine.OnStateChanged += state => OnEntityStateChanged?.Invoke(state);
            StateMachine();
        }

        protected abstract void References();
        protected abstract void StateMachine();
        protected virtual void Update() => stateMachine.Tick();
        protected virtual void FixedUpdate() => stateMachine.FixedTick();
    }
}
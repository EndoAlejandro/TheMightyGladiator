using System;
using Inputs;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerStateMachine : MonoBehaviour
    {
        #region State Machine

        public event Action<IState> OnEntityStateChanged;
        public Type CurrentStateType => _stateMachine.CurrentState.GetType();
        private StateMachine _stateMachine;

        #endregion

        [SerializeField] private float speed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float rotationSpeed = 100f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            References();

            _stateMachine = new StateMachine();
            _stateMachine.OnStateChanged += state => OnEntityStateChanged?.Invoke(state);

            var idle = new PlayerIdle(transform, _rigidbody, speed, acceleration, rotationSpeed);
            var attack = new PlayerAttack();

            _stateMachine.SetState(idle);
            _stateMachine.AddTransition(idle, attack, () => InputReader.Instance.Attack);
            _stateMachine.AddTransition(attack, idle, () => attack.Ended);
        }

        private void References() => _rigidbody = GetComponent<Rigidbody>();

        private void Update() => _stateMachine.Tick();
        private void FixedUpdate() => _stateMachine.FixedTick();
    }

    public class PlayerDodge : IState
    {
        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }

    public class PlayerShield : IState
    {
        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }

    public class PlayerAttack : IState
    {
        public bool Ended { get; private set; }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => Ended = true;
        public void OnExit() => Ended = false;
    }
}
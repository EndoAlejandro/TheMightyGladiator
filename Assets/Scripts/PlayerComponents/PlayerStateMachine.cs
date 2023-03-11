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
        public IState CurrentStateType => _stateMachine.CurrentState;
        private StateMachine _stateMachine;

        #endregion

        private Player _player;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            References();

            _stateMachine = new StateMachine();
            _stateMachine.OnStateChanged += state => OnEntityStateChanged?.Invoke(state);

            var idle = new PlayerIdle(_player, _rigidbody);
            var attack = new PlayerAttack(_player);
            var shield = new PlayerShield(_player);
            var dodge = new PlayerDodge(_player, _rigidbody);

            _stateMachine.SetState(idle);
            _stateMachine.AddTransition(idle, attack, () => InputReader.Instance.Attack && _player.CanAttack);
            _stateMachine.AddTransition(attack, idle, () => attack.Ended);

            _stateMachine.AddTransition(idle, shield, () => InputReader.Instance.Shield);
            _stateMachine.AddTransition(shield, idle, () => !InputReader.Instance.Shield);

            _stateMachine.AddTransition(idle, dodge, () => InputReader.Instance.Dodge && _player.CanDodge);
            _stateMachine.AddTransition(shield, dodge, () => InputReader.Instance.Dodge && _player.CanDodge);
            _stateMachine.AddTransition(dodge, idle, () => dodge.Ended);
        }

        private void References()
        {
            _player = GetComponent<Player>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update() => _stateMachine.Tick();
        private void FixedUpdate() => _stateMachine.FixedTick();
    }

    public class PlayerShield : IState
    {
        private readonly Player _player;

        public PlayerShield(Player player) => _player = player;

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
}
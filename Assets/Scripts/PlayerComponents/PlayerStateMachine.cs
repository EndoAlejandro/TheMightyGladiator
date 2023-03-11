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

            _stateMachine.SetState(idle);
            _stateMachine.AddTransition(idle, attack, () => _player.CanAttack && InputReader.Instance.Attack);
            _stateMachine.AddTransition(attack, idle, () => attack.Ended);

            _stateMachine.AddTransition(idle, shield, () => InputReader.Instance.Shield);
            _stateMachine.AddTransition(shield, idle, () => !InputReader.Instance.Shield);
        }

        private void References()
        {
            _player = GetComponent<Player>();
            _rigidbody = GetComponent<Rigidbody>();
        }

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

    public class PlayerAttack : IState
    {
        private const float AttackAnimDuration = 0.40f;
        private readonly Player _player;
        private float _timer;
        public bool Ended => _timer <= 0f;
        public PlayerAttack(Player player) => _player = player;

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = AttackAnimDuration;

        public void OnExit()
        {
            _player.Attack();
            _timer = 0f;
        }
    }
}
using Inputs;
using StateMachineComponents;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerStateMachine : FiniteStateBehaviour
    {
        private Player _player;
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            References();
            base.Awake();

            var idle = new PlayerIdle(_player, _rigidbody);
            var attack = new PlayerAttack(_player);
            var shield = new PlayerShield(_player, _rigidbody);
            var dodge = new PlayerDodge(_player, _rigidbody);

            stateMachine.SetState(idle);
            stateMachine.AddTransition(idle, attack, () => InputReader.Instance.Attack && _player.CanAttack);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);

            stateMachine.AddTransition(idle, shield, () => InputReader.Instance.Shield && _player.CanDefend);
            stateMachine.AddTransition(shield, idle, () => !InputReader.Instance.Shield || !_player.CanDefend);

            stateMachine.AddTransition(idle, dodge, () => InputReader.Instance.Dodge && _player.CanDodge);
            stateMachine.AddTransition(shield, dodge, () => InputReader.Instance.Dodge && _player.CanDodge);
            stateMachine.AddTransition(dodge, idle, () => dodge.Ended);
        }

        private void References()
        {
            _player = GetComponent<Player>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void Update()
        {
            if (!_player.CanMove) return;
            base.Update();
        }

        protected override void FixedUpdate()
        {
            if (!_player.CanMove) return;
            base.FixedUpdate();
        }
    }
}
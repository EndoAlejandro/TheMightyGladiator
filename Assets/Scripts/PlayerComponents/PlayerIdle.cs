using Inputs;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerIdle : IState
    {
        private readonly Player _player;
        private readonly Rigidbody _rigidbody;
        private Vector3 _moveDirection;

        public PlayerIdle(Player player, Rigidbody rigidbody)
        {
            _player = player;
            _rigidbody = rigidbody;
        }

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
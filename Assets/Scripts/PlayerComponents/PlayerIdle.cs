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
            /*var aimDirection = new Vector3(InputReader.Instance.Aim.x, 0f, InputReader.Instance.Aim.y);
            _player.transform.forward =
                Vector3.Lerp(_player.transform.forward, aimDirection, Time.deltaTime * _player.RotationSpeed);

            _moveDirection = new Vector3(InputReader.Instance.Movement.x, 0f, InputReader.Instance.Movement.y)
                .normalized;*/
        }

        public void FixedTick()
        {
            /*_rigidbody.AddForce(_moveDirection * (_player.Speed * _player.Acceleration), ForceMode.Force);

            if (_rigidbody.velocity.magnitude > _player.Speed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * _player.Speed;*/
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}
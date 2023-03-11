using Inputs;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerIdle : IState
    {
        private readonly Transform _transform;
        private readonly Rigidbody _rigidbody;
        private readonly float _speed;
        private readonly float _acceleration;
        private readonly float _rotationSpeed;

        private Vector3 _moveDirection;

        public PlayerIdle(Transform transform, Rigidbody rigidbody, float speed, float acceleration,
            float rotationSpeed)
        {
            _transform = transform;
            _rigidbody = rigidbody;
            _speed = speed;
            _acceleration = acceleration;
            _rotationSpeed = rotationSpeed;
        }

        public void Tick()
        {
            var aimDirection = new Vector3(InputReader.Instance.Aim.x, 0f, InputReader.Instance.Aim.y);
            _transform.forward =
                Vector3.Lerp(_transform.forward, aimDirection, Time.deltaTime * _rotationSpeed);

            _moveDirection = new Vector3(InputReader.Instance.Movement.x, 0f, InputReader.Instance.Movement.y)
                .normalized;
        }

        public void FixedTick()
        {
            _rigidbody.AddForce(_moveDirection * (_speed * _acceleration), ForceMode.Force);

            if (_rigidbody.velocity.magnitude > _speed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * _speed;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}
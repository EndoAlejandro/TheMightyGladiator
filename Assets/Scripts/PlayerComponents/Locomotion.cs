using Inputs;
using UnityEngine;

namespace PlayerComponents
{
    public class Locomotion : MonoBehaviour
    {
        private Vector3 _moveDirection;

        private Player _player;
        private PlayerStateMachine _playerStateMachine;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_playerStateMachine.CurrentStateType is PlayerDodge) return;

            PlayerRotation();
            _moveDirection = new Vector3(InputReader.Instance.Movement.x, 0f, InputReader.Instance.Movement.y)
                .normalized;
        }

        private void FixedUpdate()
        {
            if (_playerStateMachine.CurrentStateType is PlayerDodge) return;

            _rigidbody.AddForce(_moveDirection * (_player.Speed * _player.Acceleration), ForceMode.Force);
            if (!_player.IsImmune) SpeedControl();
        }

        private void PlayerRotation()
        {
            Vector2 playerViewPort = Camera.main.WorldToViewportPoint(transform.position);
            var viewportDirection = (InputReader.Instance.Aim - playerViewPort).normalized;
            var aimDirection = new Vector3(viewportDirection.x, 0f, viewportDirection.y);

            transform.forward =
                Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * _player.RotationSpeed);
        }

        private void SpeedControl()
        {
            if (_rigidbody.velocity.magnitude > _player.Speed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * _player.Speed;
        }
    }
}
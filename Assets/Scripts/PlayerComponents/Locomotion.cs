using CustomUtils;
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
            if (_playerStateMachine.CurrentStateType is PlayerDodge ||
                !_player.CanMove ||
                !_player.IsAlive) return;

            PlayerRotation();
            _moveDirection = (Camera.main.transform.right * InputReader.Instance.Movement.x +
                Camera.main.transform.forward * InputReader.Instance.Movement.y).With(y: 0f).normalized;
        }

        private void FixedUpdate()
        {
            if (_playerStateMachine.CurrentStateType is PlayerDodge ||
                !_player.CanMove ||
                !_player.IsAlive) return;

            _rigidbody.AddForce(_moveDirection * (_player.WalkSpeed * _player.Acceleration), ForceMode.Acceleration);
            if (!_player.IsImmune) SpeedControl();
        }

        private void PlayerRotation()
        {
            var aimDirection = Vector3.zero;
#if UNITY_EDITOR
            Vector2 playerViewPort = Camera.main.WorldToViewportPoint(transform.position);
            var viewportDirection = (InputReader.Instance.Aim - playerViewPort).normalized;
            aimDirection = new Vector3(viewportDirection.x, 0f, viewportDirection.y);
#elif UNITY_ANDROID
            aimDirection = new Vector3(InputReader.Instance.Aim.x, 0f, InputReader.Instance.Aim.y);
#endif
            transform.forward =
                Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * _player.RotationSpeed);
        }

        private void SpeedControl()
        {
            var flatVelocity = _rigidbody.linearVelocity.With(y: 0f);
            if (flatVelocity.magnitude > _player.WalkSpeed)
                _rigidbody.linearVelocity =
                    flatVelocity.normalized * _player.WalkSpeed +
                    Vector3.up * (_rigidbody.linearVelocity.y - 9.8f * Time.deltaTime);
        }
    }
}
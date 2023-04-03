using Inputs;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerDodge : IState
    {
        private const float ExitDodgeTime = 0.5f;

        private readonly Player _player;
        private readonly Rigidbody _rigidbody;

        private float _timer;
        private float _currentDistance;
        private float _lastDistance;
        private float _initialDistance;

        private Vector3 _targetPosition;
        private Vector3 _direction;

        public bool Ended { get; private set; }

        public PlayerDodge(Player player, Rigidbody rigidbody)
        {
            _player = player;
            _rigidbody = rigidbody;
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0) Ended = true;
        }

        private float GetDistance() => Vector3.Distance(_player.transform.position, _targetPosition);

        public void FixedTick()
        {
            _currentDistance = GetDistance();
            if (_currentDistance > _lastDistance) Ended = true;

            _rigidbody.AddForce(_direction * _player.DodgeSpeed * _player.Acceleration, ForceMode.Acceleration);
            _lastDistance = GetDistance();
        }

        public void OnEnter()
        {
            _player.SetDodgeState(true);
            Ended = false;

            _timer = ExitDodgeTime;

            _direction = InputReader.Instance.Movement.magnitude == 0f
                ? -_player.transform.forward
                : new Vector3(InputReader.Instance.Movement.x, 0f, InputReader.Instance.Movement.y).normalized;

            _targetPosition = _direction * _player.DodgeDistance + _player.transform.position;
            _lastDistance = GetDistance();
        }

        public void OnExit() => _player.SetDodgeState(false);
    }

    public class PlayerDeath : IState
    {
        private readonly Rigidbody _rigidbody;
        private readonly Collider _collider;

        public PlayerDeath(Rigidbody rigidbody, Collider collider)
        {
            _rigidbody = rigidbody;
            _collider = collider;
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
        }

        public void OnExit()
        {
        }
    }
}
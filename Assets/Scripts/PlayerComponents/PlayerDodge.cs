using Inputs;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerDodge : IState
    {
        private const float DodgeDistance = 2f;
        private const float DodgeSpeed = 10f;
        private const float ExitDodgeTime = 2f;

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
        }

        private float GetDistance() => Vector3.Distance(_player.transform.position, _targetPosition);

        public void FixedTick()
        {
            _timer -= Time.fixedDeltaTime;
            _currentDistance = GetDistance();
            
            if (_currentDistance > _lastDistance || _timer <= 0) 
                Ended = true;
            
            _rigidbody.AddForce(_direction * DodgeSpeed * _player.Acceleration, ForceMode.Force);
            _lastDistance = GetDistance();
        }

        public void OnEnter()
        {
            Ended = false;
            
            _timer = ExitDodgeTime;

            _direction = InputReader.Instance.Movement.magnitude == 0f
                ? -_player.transform.forward
                : new Vector3(InputReader.Instance.Movement.x, 0f, InputReader.Instance.Movement.y).normalized;

            _targetPosition = _direction * DodgeDistance + _player.transform.position;
            _lastDistance = GetDistance();
        }

        public void OnExit() => _player.Dodge();
    }
}
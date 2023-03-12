using CustomUtils;
using Enemies.BatComponents;
using PlayerComponents;
using Unity.VisualScripting;
using UnityEngine;
using IState = StateMachineComponents.IState;

namespace Enemies
{
    internal class EnemyAttack : IState
    {
        private const float DodgeDistance = 2f;
        private const float ExitDodgeTime = 2f;

        private readonly Bat _bat;
        private readonly Player _player;
        private readonly Rigidbody _rigidbody;

        private Vector3 _direction;
        private Vector3 _targetPosition;

        private float _timer;
        private float _currentDistance;
        private float _initialDistance;
        private float _lastDistance;

        private Collider[] _results;

        public bool Ended { get; private set; }

        public EnemyAttack(Bat bat, Rigidbody rigidbody, Player player)
        {
            _bat = bat;
            _player = player;
            _rigidbody = rigidbody;

            _results = new Collider[10];
        }

        public void Tick()
        {
            var size = Physics.OverlapSphereNonAlloc(_bat.transform.position + Vector3.up * 0.75f,
                0.75f, _results);

            for (int i = 0; i < size; i++)
            {
                var result = _results[i];
                if (!result.TryGetComponent(out Player player)) continue;
                player.GetHit(_bat);
                Ended = true;
            }
        }

        public void FixedTick()
        {
            _bat.SetIsAttacking(true);
            _timer -= Time.fixedDeltaTime;
            _currentDistance = GetDistance();

            if (_currentDistance > _lastDistance || _timer <= 0) Ended = true;

            _rigidbody.AddForce(_direction * (_bat.AttackSpeed * _bat.Acceleration * 2), ForceMode.Force);
            _lastDistance = GetDistance();
        }

        private float GetDistance() => Vector3.Distance(_bat.transform.position, _targetPosition);

        public void OnEnter()
        {
            Ended = false;
            _timer = ExitDodgeTime;

            _direction = Utils.NormalizedFlatDirection(_player.transform.position, _bat.transform.position);
            _targetPosition = _direction * DodgeDistance + _bat.transform.position;

            _lastDistance = GetDistance();
        }

        public void OnExit() => _bat.SetIsAttacking(false);
        
    }
}
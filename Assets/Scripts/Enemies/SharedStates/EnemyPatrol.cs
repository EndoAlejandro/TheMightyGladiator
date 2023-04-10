using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.EnemiesSharedStates
{
    public class EnemyPatrol : IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;
        private readonly Collider[] _results;
        private readonly float _x;
        private readonly float _z;

        private float _noise;

        private Vector3 _direction;
        private Vector3 _initialPosition;

        public EnemyPatrol(Enemy enemy, Rigidbody rigidbody)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;

            _x = Random.Range(0f, 10f);
            _z = Random.Range(0f, 10f);

            _results = new Collider[10];
        }

        public virtual void Tick()
        {
            _noise += Time.deltaTime * 0.15f;

            _enemy.transform.forward = Vector3.Lerp(_enemy.transform.forward, _rigidbody.velocity,
                Time.deltaTime * _enemy.RotationSpeed);

            if (_initialPosition == Vector3.zero) _initialPosition = _enemy.transform.position;

            if (Player.Instance == null) return;
            if (Vector3.Distance(Player.Instance.transform.position, _enemy.transform.position) <
                _enemy.DetectionDistance)
                _enemy.PlayerOnRange();
        }

        public virtual void FixedTick()
        {
            var distance = Vector3.Distance(_enemy.transform.position, _initialPosition);
            if (distance > 4f)
            {
                _direction = Utils.NormalizedFlatDirection(_initialPosition, _enemy.transform.position);
            }
            else
            {
                _direction.x = Mathf.Lerp(-1f, 1f, Mathf.PerlinNoise(_noise, _enemy.transform.position.x + _x));
                _direction.z = Mathf.Lerp(-1f, 1f, Mathf.PerlinNoise(_noise, _enemy.transform.position.z + _z));
            }

            _rigidbody.AddForce(_direction.normalized * (_enemy.Speed), ForceMode.Acceleration);
        }

        public virtual void OnEnter() => _initialPosition = _enemy.transform.position;

        public virtual void OnExit()
        {
            var size = Physics.OverlapSphereNonAlloc(_enemy.transform.position, _enemy.DetectionDistance, _results);
            for (int i = 0; i < size; i++)
                if (_results[i].TryGetComponent(out Enemy enemy))
                    enemy.PlayerOnRange();
        }

        public override string ToString() => "Patrol";
    }
}
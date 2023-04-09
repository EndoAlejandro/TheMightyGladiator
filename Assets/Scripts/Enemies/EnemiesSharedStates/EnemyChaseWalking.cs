using CustomUtils;
using NavigationSteeringComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.EnemiesSharedStates
{
    public class EnemyChaseWalking : IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;
        private readonly NavigationSteering _navigationSteering;

        private Vector3 _direction;
        private float _angleVision;

        public bool PlayerOnRange { get; private set; }
        public bool CanSeePlayer { get; private set; }
        public bool PlayerInFront { get; private set; }

        public EnemyChaseWalking(Enemy enemy, Rigidbody rigidbody, NavigationSteering navigationSteering)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;
            _navigationSteering = navigationSteering;
        }

        public void Tick()
        {
            _direction = _navigationSteering.BestDirection.direction;
            _enemy.transform.forward =
                Vector3.Lerp(_enemy.transform.forward, _direction, _enemy.RotationSpeed * Time.deltaTime);

            var playerDirection =
                Utils.NormalizedFlatDirection(Player.Instance.transform.position, _enemy.transform.position);
            _angleVision = Vector3.Dot(_enemy.transform.forward, playerDirection);
            PlayerInFront = _angleVision > 0.95f;
        }

        public void FixedTick()
        {
            var batPosition = _enemy.transform.position;
            var playerPosition = Player.Instance.transform.position;

            var distance = Vector3.Distance(batPosition, playerPosition);
            CanSeePlayer = !Physics.Linecast(batPosition, playerPosition);
            PlayerOnRange = distance <= _enemy.StoppingDistance;

            if (!PlayerOnRange)
                _rigidbody.AddForce(_direction * (_enemy.Speed * _enemy.Acceleration), ForceMode.Acceleration);
            else if (distance <= _enemy.StoppingDistance - _enemy.StoppingDistance * 0.75f)
                _rigidbody.AddForce(-_direction * (_enemy.Speed * _enemy.Acceleration), ForceMode.Acceleration);
        }

        public void OnEnter()
        {
            CanSeePlayer = false;
            PlayerOnRange = false;
            PlayerInFront = false;
        }

        public void OnExit()
        {
            CanSeePlayer = false;
            PlayerOnRange = false;
            PlayerInFront = false;
        }

        public override string ToString() => "Chase";
    }
}
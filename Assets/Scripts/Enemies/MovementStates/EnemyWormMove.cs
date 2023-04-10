using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.EnemiesSharedStates
{
    public class EnemyWormPush : StateTimer, IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;

        public EnemyWormPush(Enemy enemy, Rigidbody rigidbody)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            timer = _enemy.RecoverTime;

            _rigidbody.AddForce(_enemy.transform.forward * (_enemy.Speed * _enemy.Acceleration),
                ForceMode.VelocityChange);
        }
    }

    public class EnemyWormRecover : StateTimer, IState
    {
        private readonly Enemy _enemy;
        private Vector3 _direction;
        private Vector3 _playerDirection;

        public bool PlayerOnRange { get; private set; }
        public bool PlayerInFront { get; private set; }

        public EnemyWormRecover(Enemy enemy) => _enemy = enemy;

        public override void Tick()
        {
            base.Tick();
            _direction = Utils.FindBestDirection(_enemy).direction;

            _playerDirection = Utils.FlatDirection(Player.Instance.transform.position, _enemy.transform.position);

            PlayerOnRange = _playerDirection.magnitude < _enemy.DetectionDistance;
            PlayerInFront = Vector3.Dot(_playerDirection.normalized, _enemy.transform.forward) > 0.97f;

            if (!PlayerInFront)
                _enemy.transform.forward =
                    Vector3.Lerp(_enemy.transform.forward, _direction, Time.deltaTime * _enemy.RotationSpeed);
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => timer = _enemy.ChaseTime;
    }
}
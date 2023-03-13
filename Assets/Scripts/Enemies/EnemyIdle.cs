using CustomUtils;
using Enemies.BatComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemyIdle : IState
    {
        private readonly Bat _bat;
        private readonly Rigidbody _rigidbody;
        private readonly Player _player;
        private readonly NavigationSteering _navigationSteering;

        private Vector3 _direction;

        private float _timer;

        public bool PlayerOnRange { get; private set; }
        public bool Ended => _timer <= 0f;

        public EnemyIdle(Bat bat, Rigidbody rigidbody, Player player, NavigationSteering navigationSteering)
        {
            _bat = bat;
            _rigidbody = rigidbody;
            _player = player;
            _navigationSteering = navigationSteering;
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;

            _direction = _navigationSteering.BestDirection.direction; //Utils.NormalizedFlatDirection(_player.transform.position, _bat.transform.position);
            _bat.transform.forward =
                Vector3.Lerp(_bat.transform.forward, _direction, _bat.RotationSpeed * Time.deltaTime);
        }

        public void FixedTick()
        {
            var distance = Vector3.Distance(_bat.transform.position, _player.transform.position);
            PlayerOnRange = distance <= _bat.StoppingDistance;

            if (!PlayerOnRange)
                _rigidbody.AddForce(_direction * (_bat.Speed * _bat.Acceleration), ForceMode.Force);
        }

        public void OnEnter()
        {
            _timer = _bat.StunTime;
            PlayerOnRange = false;
        }

        public void OnExit()
        {
        }
    }
}
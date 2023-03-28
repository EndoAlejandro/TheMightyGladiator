using PlayerComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class BatIdle : EnemyIdle
    {
        private readonly Bat _bat;
        private readonly Rigidbody _rigidbody;

        private readonly Player _player;
        private readonly NavigationSteering _navigationSteering;

        private Vector3 _direction;
        private float _timer;

        public bool PlayerOnRange { get; private set; }
        public bool CanSeePlayer { get; private set; }
        public bool Ended => _timer <= 0f;

        public BatIdle(Bat bat, Rigidbody rigidbody, Player player, NavigationSteering navigationSteering)
        {
            _bat = bat;
            _rigidbody = rigidbody;
            _player = player;
            _navigationSteering = navigationSteering;
        }

        public override void Tick()
        {
            _timer -= Time.deltaTime;
            _direction = _navigationSteering.BestDirection.direction;
            _bat.transform.forward =
                Vector3.Lerp(_bat.transform.forward, _direction, _bat.RotationSpeed * Time.deltaTime);
        }

        public override void FixedTick()
        {
            var distance = Vector3.Distance(_bat.transform.position, _player.transform.position);
            CanSeePlayer = !Physics.Linecast(_bat.transform.position, _player.transform.position);
            PlayerOnRange = distance <= _bat.StoppingDistance;

            if (!PlayerOnRange)
                _rigidbody.AddForce(_direction * (_bat.Speed * _bat.Acceleration), ForceMode.Force);
            else
                _rigidbody.AddForce(-_direction * (_bat.Speed * _bat.Acceleration), ForceMode.Force);
        }

        public override void OnEnter()
        {
            _timer = _bat.IdleTime;
            CanSeePlayer = false;
            PlayerOnRange = false;
        }

        public override void OnExit()
        {
        }
    }
}
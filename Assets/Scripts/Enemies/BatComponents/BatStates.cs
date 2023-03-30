using CustomUtils;
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
            var batPosition = _bat.transform.position;
            var playerPosition = _player.transform.position;

            var distance = Vector3.Distance(batPosition, playerPosition);
            CanSeePlayer = !Physics.Linecast(batPosition, playerPosition);
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

    public class BatAttack : EnemyAttack
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

        private readonly Collider[] _results;

        public bool Ended { get; private set; }

        public BatAttack(Bat bat, Rigidbody rigidbody, Player player) : base(bat)
        {
            _bat = bat;
            _player = player;
            _rigidbody = rigidbody;

            _results = new Collider[10];
        }

        public override void Tick()
        {
            base.Tick();
            var size = Physics.OverlapSphereNonAlloc(_bat.transform.position + Vector3.up * 0.75f,
                0.75f, _results);

            for (int i = 0; i < size; i++)
            {
                var result = _results[i];
                if (!result.TryGetComponent(out Player player)) continue;
                if (!player.GetDamageFromEnemy(_bat.transform.position)) continue;
                player.TakeDamage(_bat.transform.position, 1);
                Ended = true;
            }
        }

        public override void FixedTick()
        {
            _timer -= Time.fixedDeltaTime;
            _currentDistance = GetDistance();

            if (_currentDistance > _lastDistance || _timer <= 0) Ended = true;

            _rigidbody.AddForce(_direction * (_bat.AttackSpeed * _bat.Acceleration * 2), ForceMode.Force);
            _lastDistance = GetDistance();
        }

        private float GetDistance() => Vector3.Distance(_bat.transform.position, _targetPosition);

        public override void OnEnter()
        {
            base.OnEnter();
            _bat.SetIsAttacking(true);
            Ended = false;
            _timer = ExitDodgeTime;

            var position = _bat.transform.position;
            _direction = Utils.NormalizedFlatDirection(_player.transform.position, position);
            _targetPosition = _direction * DodgeDistance + position;

            _lastDistance = GetDistance();
        }

        public override void OnExit()
        {
            _bat.SetIsAttacking(false);
            _rigidbody.velocity = Vector3.zero;
        }
    }

    public class BatTelegraph : EnemyTelegraph
    {
        private readonly Player _player;
        private Vector3 _direction;

        public BatTelegraph(Bat bat, Player player) : base(bat) => _player = player;

        public override void Tick()
        {
            base.Tick();
            _direction = Utils.NormalizedFlatDirection(_player.transform.position, enemy.transform.position);
            enemy.transform.forward =
                Vector3.Lerp(enemy.transform.forward, _direction, enemy.RotationSpeed * Time.deltaTime);
        }
    }
}
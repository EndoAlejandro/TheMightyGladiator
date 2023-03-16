using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
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
                if (player.TryToDealDamage(_bat.transform.position))
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

            _direction = Utils.NormalizedFlatDirection(_player.transform.position, _bat.transform.position);
            _targetPosition = _direction * DodgeDistance + _bat.transform.position;

            _lastDistance = GetDistance();
        }

        public override void OnExit()
        {
            _bat.SetIsAttacking(false);
            _rigidbody.velocity = Vector3.zero;
        }
    }
}
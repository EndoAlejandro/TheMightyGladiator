using CustomUtils;
using Enemies.EnemiesSharedStates;
using FxComponents;
using NavigationSteeringComponents;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class BatAttack : EnemyAttack
    {
        private readonly Bat _bat;
        private readonly Rigidbody _rigidbody;

        private Vector3 _targetPosition;

        private float _timer;
        private float _initialDistance;

        private readonly Collider[] _results;

        public bool Ended { get; private set; }

        public BatAttack(Bat bat, Rigidbody rigidbody) : base(bat)
        {
            _bat = bat;
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
                _rigidbody.velocity = Vector3.zero;
            }
        }

        public override void FixedTick()
        {
            _timer -= Time.fixedDeltaTime;

            if (_timer <= 0) Ended = true;

            _rigidbody.AddForce(_bat.transform.forward * (_bat.AttackSpeed * _bat.Acceleration * 2),
                ForceMode.Acceleration);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _bat.SetIsAttacking(true);
            Ended = false;
            _timer = _bat.AttackTime;
            SfxManager.Instance.PlayFx(Sfx.JumpStart, _bat.transform.position);
        }

        public override void OnExit()
        {
            _bat.SetIsAttacking(false);
            _rigidbody.velocity = Vector3.zero;
            SfxManager.Instance.PlayFx(Sfx.JumpEnd, _bat.transform.position);
        }
    }

    public class BatTelegraph : EnemyTelegraph
    {
        private readonly Bat _bat;
        private Vector3 _direction;

        public BatTelegraph(Bat bat) : base(bat) => _bat = bat;

        public override void Tick()
        {
            base.Tick();
            _direction = Utils.NormalizedFlatDirection(Player.Instance.transform.position, enemy.transform.position);
            enemy.transform.forward =
                Vector3.Lerp(enemy.transform.forward, _direction, enemy.RotationSpeed * Time.deltaTime);
        }
    }
}
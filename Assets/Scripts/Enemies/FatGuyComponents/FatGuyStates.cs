using System;
using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using UnityEngine.VFX;
using VfxComponents;

namespace Enemies.FatGuyComponents
{
    public class FatGuyIdle : IState
    {
        private readonly FatGuy _fatGuy;
        private readonly Rigidbody _rigidbody;

        private Vector3 _direction;
        private float _distance;
        private float _angleVision;

        public bool Ended { get; private set; }

        public FatGuyIdle(FatGuy fatGuy, Rigidbody rigidbody)
        {
            _fatGuy = fatGuy;
            _rigidbody = rigidbody;
        }

        public void Tick()
        {
            var playerPosition = Player.Instance.transform.position;
            _direction = Utils.NormalizedFlatDirection(playerPosition, _fatGuy.transform.position);

            _fatGuy.transform.forward = Vector3.Lerp(_fatGuy.transform.forward, _direction,
                Time.deltaTime * _fatGuy.RotationSpeed);

            _distance = Vector3.Distance(playerPosition, _fatGuy.transform.position);
            _angleVision = Vector3.Dot(_fatGuy.transform.forward, _direction);
            if (_distance < _fatGuy.StoppingDistance && _angleVision > 0.97f) Ended = true;
        }

        public void FixedTick()
        {
            if (_distance >= _fatGuy.StoppingDistance)
                _rigidbody.AddForce(_direction * _fatGuy.Acceleration * _fatGuy.Speed, ForceMode.Acceleration);
        }

        public void OnEnter() => Ended = false;
        public void OnExit() => Ended = false;
    }

    public class FatGuyTelegraph : EnemyTelegraph
    {
        private readonly Action _fx;
        public FatGuyTelegraph(FatGuy fatGuy, Action fx = null) : base(fatGuy) => _fx = fx;

        public override void OnEnter()
        {
            base.OnEnter();
            _fx?.Invoke();
        }
    }

    public class FatGuyRecover : EnemyRecover
    {
        private readonly Action _nextAttackPattern;

        public FatGuyRecover(Enemy enemy, Action nextAttackPattern) : base(enemy)
        {
            _nextAttackPattern = nextAttackPattern;
        }

        public override void OnExit()
        {
            base.OnExit();
            _nextAttackPattern?.Invoke();
        }
    }

    public class FatGuyDashAttack : EnemyAttack
    {
        private readonly FatGuy _fatGuy;
        private readonly Rigidbody _rigidbody;
        private readonly Collider[] _results;

        private float _timer;

        public FatGuyDashAttack(FatGuy fatGuy, Rigidbody rigidbody) : base(fatGuy)
        {
            _fatGuy = fatGuy;
            _rigidbody = rigidbody;

            _results = new Collider[10];
        }

        public bool Ended { get; private set; }

        public override void Tick()
        {
            base.Tick();
            var size = Physics.OverlapSphereNonAlloc(_fatGuy.transform.position + Vector3.up * 1.5f,
                1.5f, _results);

            for (int i = 0; i < size; i++)
            {
                if (_results[i].TryGetComponent(out Player player))
                    _rigidbody.velocity = Vector3.zero;
            }
        }

        public override void FixedTick()
        {
            _timer -= Time.fixedDeltaTime;

            if (_timer <= 0f) Ended = true;

            _rigidbody.AddForce(_fatGuy.transform.forward * (20f * _fatGuy.Acceleration * 2),
                ForceMode.Acceleration);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Ended = false;
            _timer = 0.25f;
        }
    }

    public class FatGuyAoEAttack : IState
    {
        private readonly FatGuy _fatGuy;

        private Collider[] _results;

        public FatGuyAoEAttack(FatGuy fatGuy)
        {
            _fatGuy = fatGuy;
            _results = new Collider[40];
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            var position = _fatGuy.transform.position + Vector3.up * 1.5f;
            VfxManager.Instance.PlayFx(Vfx.Explosion, position);
            MainCamera.Instance.Shake(2f);

            var size = Physics.OverlapSphereNonAlloc(position, _fatGuy.StoppingDistance, _results);

            for (int i = 0; i < size; i++)
            {
                if (_results[i].TryGetComponent(out Player player))
                    player.TryToGetDamageFromEnemy(_fatGuy, true);
            }
        }

        public void OnExit()
        {
        }
    }
}
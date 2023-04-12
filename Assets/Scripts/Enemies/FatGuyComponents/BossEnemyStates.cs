using System;
using CustomUtils;
using Enemies.EnemiesSharedStates;
using FxComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies.FatGuyComponents
{
    public class BossIdle : IState
    {
        public override string ToString() => "Idle";

        private readonly BossEnemy _bossEnemy;
        private readonly Rigidbody _rigidbody;

        private Vector3 _direction;
        private float _distance;
        private float _angleVision;

        public bool Ended { get; private set; }

        public BossIdle(BossEnemy bossEnemy, Rigidbody rigidbody)
        {
            _bossEnemy = bossEnemy;
            _rigidbody = rigidbody;
        }

        public void Tick()
        {
            var playerPosition = Player.Instance.transform.position;
            _direction = Utils.NormalizedFlatDirection(playerPosition, _bossEnemy.transform.position);

            _bossEnemy.transform.forward = Vector3.Lerp(_bossEnemy.transform.forward, _direction,
                Time.deltaTime * _bossEnemy.RotationSpeed);

            _distance = Vector3.Distance(playerPosition, _bossEnemy.transform.position);
            _angleVision = Vector3.Dot(_bossEnemy.transform.forward, _direction);
            if (_distance < _bossEnemy.StoppingDistance && _angleVision > 0.97f) Ended = true;
        }

        public void FixedTick()
        {
            if (_distance >= _bossEnemy.StoppingDistance)
                _rigidbody.AddForce(_direction * _bossEnemy.Acceleration * _bossEnemy.Speed, ForceMode.Acceleration);
        }

        public void OnEnter() => Ended = false;
        public void OnExit() => Ended = false;
    }

    public class BossTelegraph : EnemyTelegraph
    {
        private readonly Action _fx;
        private readonly float _customTime;

        public BossTelegraph(BossEnemy bossEnemy, Action fx = null, float customTime = -1f) : base(bossEnemy)
        {
            _fx = fx;
            _customTime = customTime;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (_customTime > 0) timer = _customTime;
            _fx?.Invoke();
        }
    }

    public class BossRecover : EnemyRecover
    {
        private readonly Action _nextAttackPattern;

        public BossRecover(Enemy enemy, Action nextAttackPattern) : base(enemy)
        {
            _nextAttackPattern = nextAttackPattern;
        }

        public override void OnExit()
        {
            base.OnExit();
            _nextAttackPattern?.Invoke();
        }
    }

    public class BossDashAttack : EnemyAttack
    {
        private readonly BossEnemy _bossEnemy;
        private readonly Rigidbody _rigidbody;
        private readonly Collider[] _results;

        private float _timer;

        public BossDashAttack(BossEnemy bossEnemy, Rigidbody rigidbody) : base(bossEnemy)
        {
            _bossEnemy = bossEnemy;
            _rigidbody = rigidbody;

            _results = new Collider[10];
        }

        public bool Ended { get; private set; }

        public override void Tick()
        {
            base.Tick();
            var size = Physics.OverlapSphereNonAlloc(_bossEnemy.transform.position + Vector3.up * 1.5f,
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

            _rigidbody.AddForce(_bossEnemy.transform.forward * (20f * _bossEnemy.Acceleration * 2),
                ForceMode.Acceleration);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Ended = false;
            _timer = 0.25f;
            SfxManager.Instance.PlayFx(Sfx.JumpStart, _bossEnemy.transform.position);
        }

        public override void OnExit()
        {
            base.OnExit();
            SfxManager.Instance.PlayFx(Sfx.JumpEnd, _bossEnemy.transform.position);
        }
    }

    public class BossAoeAttack : IState
    {
        public override string ToString() => "Attack";

        private readonly BossEnemy _bossEnemy;
        private readonly Collider[] _results;

        public BossAoeAttack(BossEnemy bossEnemy)
        {
            _bossEnemy = bossEnemy;
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
            var position = _bossEnemy.transform.position + Vector3.up * 1.5f;
            VfxManager.Instance.PlayFx(Vfx.Explosion, position);
            SfxManager.Instance.PlayFx(Sfx.MortarShot, position);
            MainCamera.Instance.Shake(2f);

            var size = Physics.OverlapSphereNonAlloc(position, _bossEnemy.StoppingDistance, _results);

            for (int i = 0; i < size; i++)
            {
                if (_results[i].TryGetComponent(out Player player))
                    player.TryToGetDamageFromEnemy(_bossEnemy, true);
            }
        }

        public void OnExit()
        {
        }
    }

    public class BossShot : IState
    {
        private readonly BossEnemy _bossEnemy;
        private Vector3[] _directions;
        public BossShot(BossEnemy bossEnemy) => _bossEnemy = bossEnemy;

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _directions = Utils.GetFanPatternDirections(_bossEnemy.transform, _bossEnemy.BulletsAmount, 360f);
            SfxManager.Instance.PlayFx(Sfx.MortarShot, _bossEnemy.transform.position);

            foreach (var direction in _directions)
            {
                var bullet = _bossEnemy.BulletPrefab.Get<Bullet>(_bossEnemy.transform.position + Vector3.up,
                    Quaternion.Euler(direction));
                bullet.Setup(direction, 10f, _bossEnemy.Damage);
            }
        }

        public void OnExit()
        {
        }
    }

    public class BossDeath : EnemyDeath
    {
        private readonly BossEnemy _boss;
        private NavMeshHit _hit;

        public BossDeath(BossEnemy boss, Rigidbody rigidbody, Collider collider) : base(boss, rigidbody, collider) =>
            _boss = boss;

        public override void OnExit()
        {
            base.OnExit();
            var position = Random.insideUnitSphere.With(y: 0f).normalized * 4f;
            NavMesh.SamplePosition(position, out _hit, 1f, NavMesh.AllAreas);
            GameObject.Instantiate(_boss.HealPrefab, _hit.position, Quaternion.identity);

            var upgrades = _boss.UpgradePrefabs;
            var randomIndex = Random.Range(0, upgrades.Length);
            position = Random.insideUnitSphere.With(y: 0f).normalized * 4f;
            NavMesh.SamplePosition(position, out _hit, 1f, NavMesh.AllAreas);
            GameObject.Instantiate(upgrades[randomIndex], _hit.position, Quaternion.identity);
        }
    }
}
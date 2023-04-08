using CustomUtils;
using NavigationSteeringComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawn : StateTimer, IState
    {
        public override string ToString() => "Spawn";

        public void FixedTick()
        {
        }

        public void OnEnter() => timer = Constants.SPAWN_TIME;
    }

    public class EnemyStun : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyStun(Enemy enemy) => _enemy = enemy;

        public void OnEnter()
        {
            timer = _enemy.StunTime;
            _enemy.SetIsStun(true);
        }

        public override string ToString() => "Stun";

        public void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            _enemy.SetIsStun(false);
        }
    }

    public class EnemyGetHit : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyGetHit(Enemy enemy) => _enemy = enemy;
        public virtual void OnEnter() => timer = _enemy.GetHitTime;
        public override string ToString() => "GetHit";

        public virtual void FixedTick()
        {
        }
    }

    public class EnemyRecover : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyRecover(Enemy enemy) => _enemy = enemy;
        public virtual void OnEnter() => timer = _enemy.RecoverTime;
        public override string ToString() => "Recover";

        public virtual void FixedTick()
        {
        }
    }

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

        public override string ToString() => "Idle";
    }

    public class EnemyIdle : IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;
        private readonly NavigationSteering _navigationSteering;

        private Vector3 _direction;

        public bool PlayerOnRange { get; private set; }
        public bool CanSeePlayer { get; private set; }

        public EnemyIdle(Enemy enemy, Rigidbody rigidbody, NavigationSteering navigationSteering)
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
        }

        public void OnExit()
        {
        }

        public override string ToString() => "Idle";
    }

    public class EnemyTelegraph : StateTimer, IState
    {
        protected readonly Enemy enemy;
        public EnemyTelegraph(Enemy enemy) => this.enemy = enemy;
        public virtual void OnEnter() => timer = enemy.TelegraphTime;
        public override string ToString() => "Telegraph";

        public virtual void FixedTick()
        {
        }
    }

    public class EnemyDeath : StateTimer, IState
    {
        public override string ToString() => "Death";
        protected readonly Enemy enemy;
        public EnemyDeath(Enemy enemy) => this.enemy = enemy;
        public virtual void OnEnter() => timer = enemy.DeathTime;

        public virtual void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            enemy.DeSpawn();
        }
    }

    public abstract class EnemyAttack : IState
    {
        protected readonly Enemy enemy;
        private float _parryTimer;
        public EnemyAttack(Enemy enemy) => this.enemy = enemy;

        public virtual void Tick()
        {
            _parryTimer -= Time.deltaTime;
            if (_parryTimer <= 0) enemy.SetCanBeParried(false);
        }

        public abstract void FixedTick();

        public virtual void OnEnter()
        {
            _parryTimer = enemy.ParryTimeWindow;
            enemy.SetCanBeParried(true);
            enemy.SetIsAttacking(true);
        }

        public virtual void OnExit()
        {
            enemy.SetCanBeParried(false);
            enemy.SetIsAttacking(false);
        }

        public override string ToString() => "Attack";
    }
}
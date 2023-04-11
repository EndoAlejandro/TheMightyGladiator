using CustomUtils;
using FxComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.EnemiesSharedStates
{
    public class EnemyInitialJump : IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;
        private readonly Collider _collider;

        public bool Ended { get; private set; }

        public EnemyInitialJump(Enemy enemy, Rigidbody rigidbody, Collider collider)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;
            _collider = collider;
        }

        public void Tick()
        {
            _enemy.transform.position += Vector3.up * (Constants.JUMP_SPEED * Time.deltaTime);
            if (_enemy.transform.position.y > Constants.JUMP_HEIGHT) Ended = true;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _rigidbody.isKinematic = true;
            Ended = false;
            _collider.enabled = false;
            VfxManager.Instance.PlayFx(Vfx.SplashJump, _enemy.transform.position);
            SfxManager.Instance.PlayFx(Sfx.JumpStart, _enemy.transform.position);
        }

        public void OnExit() => _enemy.transform.position = _enemy.transform.position.With(y: Constants.JUMP_HEIGHT);
    }

    public class EnemyOnAirJump : StateTimer, IState
    {
        private readonly Enemy _enemy;

        private Vector3 _direction;

        public EnemyOnAirJump(Enemy enemy) => _enemy = enemy;

        public override void Tick()
        {
            base.Tick();
            _direction = Utils.FlatDirection(Player.Instance.transform.position, _enemy.transform.position);
            _enemy.transform.forward =
                Vector3.Lerp(_enemy.transform.forward, _direction, _enemy.RotationSpeed * Time.deltaTime);

            if (_direction.magnitude < 0.5f) return;
            _enemy.transform.position += _direction.normalized * (_enemy.Speed * _enemy.Acceleration * Time.deltaTime);
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => timer = _enemy.ChaseTime;
    }

    public class EnemyEndJump : IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;
        private readonly Collider _collider;

        public bool Ended { get; private set; }

        public EnemyEndJump(Enemy enemy, Rigidbody rigidbody, Collider collider)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;
            _collider = collider;
        }

        public void Tick()
        {
            _enemy.transform.position += Vector3.down * (Constants.JUMP_SPEED * Time.deltaTime);
            if (_enemy.transform.position.y <= 0f) Ended = true;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            Ended = false;
            _rigidbody.isKinematic = true;
        }

        public void OnExit()
        {
            _enemy.transform.position = _enemy.transform.position.With(y: 0f);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
        }
    }
}
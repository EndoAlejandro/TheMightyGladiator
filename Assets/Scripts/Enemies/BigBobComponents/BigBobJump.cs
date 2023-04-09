using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBobInitialJump : IState
    {
        private readonly BigBob _bigBob;
        private readonly Rigidbody _rigidbody;

        private const float JumpHeight = 15f;

        public bool Ended { get; private set; }

        public BigBobInitialJump(BigBob bigBob, Rigidbody rigidbody)
        {
            _bigBob = bigBob;
            _rigidbody = rigidbody;
        }

        public void Tick()
        {
            if (_bigBob.transform.position.y < JumpHeight)
                _bigBob.transform.position += Vector3.up * (50 * Time.deltaTime);
            else
                Ended = true;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _rigidbody.isKinematic = true;
            Ended = false;
        }

        public void OnExit() =>
            _bigBob.transform.position =
                new Vector3(_bigBob.transform.position.x, JumpHeight, _bigBob.transform.position.z);
    }

    public class BigBobAirJump : IState
    {
        private readonly BigBob _bigBob;
        private readonly Player _player;

        private const float Speed = 15f;
        private float _timer;

        public bool Ended => _timer <= 0f;

        public BigBobAirJump(BigBob bigBob, Player player)
        {
            _bigBob = bigBob;
            _player = player;
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;

            var distance = Vector3.Distance(_player.transform.position, _bigBob.transform.position);

            if (distance < 0.5f) return;
            var direction = Utils.NormalizedFlatDirection(_player.transform.position, _bigBob.transform.position);
            _bigBob.transform.position += direction * Speed * Time.deltaTime;
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = 5f;

        public void OnExit()
        {
        }
    }

    public class BigBobEndJump : IState
    {
        private const float Speed = 50f;

        private readonly BigBob _bigBob;
        private readonly Rigidbody _rigidbody;
        private readonly Collider[] _results;

        public bool Ended { get; private set; }

        public BigBobEndJump(BigBob bigBob, Rigidbody rigidbody)
        {
            _bigBob = bigBob;
            _rigidbody = rigidbody;

            _results = new Collider[10];
        }

        public void Tick()
        {
            if (_bigBob.transform.position.y < 0) Ended = true;

            _bigBob.transform.position += Vector3.down * Speed * Time.deltaTime;

            var size = Physics.OverlapSphereNonAlloc(_bigBob.transform.position + Vector3.up * _bigBob.YOffset,
                _bigBob.AoeRadius, _results);

            for (int i = 0; i < size; i++)
            {
                var result = _results[i];
                if (!result.TryGetComponent(out Player player)) continue;
                player.TryToGetDamageFromEnemy(_bigBob);
            }
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            Ended = false;
            _bigBob.SetIsAttacking(true);
        }

        public void OnExit()
        {
            _bigBob.SetIsAttacking(false);
            _bigBob.transform.position = new Vector3(_bigBob.transform.position.x, 0f, _bigBob.transform.position.z);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = false;
        }
    }
}
using CustomUtils;
using Enemies.BatComponents;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAttack : IState
    {
        private const float AttackAnimDuration = 0.5f;

        private readonly Player _player;
        private float _timer;
        private bool _triggered;

        private readonly float _hitBoxSize;
        private readonly Vector3 _offset;

        private readonly Collider[] _results;

        public bool Ended => _timer <= 0f;

        public PlayerAttack(Player player)
        {
            _player = player;

            _hitBoxSize = _player.HitBoxSize;
            _offset = Vector3.up * _player.Height;

            _results = new Collider[20];
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;

            if (_triggered) return;
            if (_timer <= AttackAnimDuration / 2)
            {
                _triggered = true;
                AttackDamage();
            }
        }

        private void AttackDamage()
        {
            var size = Physics.OverlapSphereNonAlloc(_player.transform.position + _offset,
                _hitBoxSize,
                _results,
                _player.AttackLayerMask);

            for (int i = 0; i < size; i++)
            {
                var result = _results[i];

                if (!result.TryGetComponent(out Bat bat)) continue;
                if (!bat.IsAlive) return;
                var direction = Utils.NormalizedFlatDirection(result.transform.position, _player.transform.position);
                var angle = Vector3.Angle(direction, _player.transform.forward);
                if (angle > _player.AttackAngle) continue;

                bat.GetHit(_player);
                _player.DealDamage(result.ClosestPoint(_player.transform.position));
                CamShake.Instance.Shake();
            }
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _triggered = false;
            _timer = AttackAnimDuration;
        }

        public void OnExit()
        {
            _player.Attack();
            _timer = 0f;
        }
    }
}
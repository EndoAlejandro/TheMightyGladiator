using CustomUtils;
using Enemies.BatComponents;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerShield : IState
    {
        private readonly Player _player;
        private readonly Rigidbody _rigidbody;

        private readonly Collider[] _results;
        private readonly Vector3 _offset;
        private float _parryTime;

        public bool Ended { get; private set; }

        public PlayerShield(Player player, Rigidbody rigidbody)
        {
            _player = player;
            _rigidbody = rigidbody;

            _offset = Vector3.up * _player.Height;
            _results = new Collider[20];
        }

        public void Tick()
        {
            if (_parryTime > 0f) _parryTime -= Time.deltaTime;

            var size = Physics.OverlapSphereNonAlloc(_player.transform.position + _offset,
                _player.DefendBoxSize,
                _results, _player.AttackLayerMask);


            for (int i = 0; i < size; i++)
            {
                var result = _results[i];

                if (!result.TryGetComponent(out Bat bat)) continue;
                if (bat.IsOnKnockBack) continue;
                var direction = Utils.NormalizedFlatDirection(bat.transform.position, _player.transform.position);
                var angle = Vector3.Angle(direction, _player.transform.forward);
                if (angle > _player.DefendAngle) continue;

                if (bat.IsAttacking && _parryTime > 0)
                {
                    _player.Parry();
                    Ended = true;
                    bat.Parry(_player);
                }
                else
                    _player.ShieldHit();

                bat.KnockBack(_player);
                CamShake.Instance.Shake();
            }
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _parryTime = _player.ParryTime;
            Ended = false;
        }

        public void OnExit()
        {
        }
    }
}
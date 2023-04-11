using CustomUtils;
using Enemies;
using Enemies.BatComponents;
using FxComponents;
using StateMachineComponents;
using UnityEngine;
using VfxComponents;

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
            if (_parryTime < 0) return;

            _parryTime -= Time.deltaTime;

            var size = Physics.OverlapSphereNonAlloc(_player.transform.position + _offset,
                _player.DefendBoxSize,
                _results, _player.AttackLayerMask);


            for (int i = 0; i < size; i++)
            {
                var result = _results[i];
                if (result.TryGetComponent(out Enemy enemy))
                {
                    if (!enemy.IsAttacking || !enemy.CanBeParried) continue;
                    enemy.Parry(_player);
                    PlayerParry();
                    SfxManager.Instance.PlayFx(Sfx.ShieldHit, _player.transform.position);
                    VfxManager.Instance.PlayFx(Vfx.SwordCritical,
                        _player.transform.position + Vector3.up + _player.transform.forward * 0.5f);
                }
                else if (result.TryGetComponent(out Bullet bullet))
                {
                    bullet.Parry();
                    PlayerParry();
                    SfxManager.Instance.PlayFx(Sfx.ShieldHit, _player.transform.position);
                    VfxManager.Instance.PlayFx(Vfx.SwordCritical,
                        _player.transform.position + Vector3.up + _player.transform.forward * 0.5f);
                }
            }
        }

        private void PlayerParry()
        {
            _player.Parry();
            _parryTime = -1;
            Ended = true;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _player.SetShieldActive(true);
            _parryTime = _player.ParryTime;
            Ended = false;
        }

        public void OnExit() => _player.SetShieldActive(false);
    }
}
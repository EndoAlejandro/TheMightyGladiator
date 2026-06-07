using System;
using FxComponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerComponents
{
    [RequireComponent(typeof(Collider))]
    public class SwordHitbox : MonoBehaviour
    {
        private Player _player;
        private Collider _collider;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            PlayerAttackOnAttackUpdated(false);
            _renderer.flipY = true;
        }

        private void Flip() => _renderer.flipY = !_renderer.flipY;

        private void OnEnable() => PlayerAttack.OnAttackUpdated += PlayerAttackOnAttackUpdated;
        private void OnDisable() => PlayerAttack.OnAttackUpdated -= PlayerAttackOnAttackUpdated;

        private void PlayerAttackOnAttackUpdated(bool isAttacking)
        {
            if (_collider == null) return;

            _collider.enabled = isAttacking;

            if (isAttacking)
            {
                Flip();
                TileWorldController.Instance.TryToAttackWalls(_collider.bounds.center, 1f);
            }
            else
            {
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable enemy)) return;
            AttackEnemy(enemy);
        }

        private void AttackEnemy(IDamageable enemy)
        {
            if (!enemy.IsAlive) return;

            var multiplier = 1f;
            var fx = Vfx.Sword;
            var isCritical = enemy.IsStun || Random.Range(0f, 1f) < _player.CriticalProbability;
            if (isCritical)
            {
                multiplier = _player.CriticalDamage;
                fx = Vfx.SwordCritical;
            }

            enemy.TakeDamage(transform.position, _player.Damage * multiplier,
                _player.KnockBackForce);
            VfxManager.Instance.PlayFx(fx, enemy.transform.position);
            MainCamera.Instance.Shake(isCritical ? 1 : 0.5f);
        }
    }
}
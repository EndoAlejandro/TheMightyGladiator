using System;
using PlayerComponents;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies.BigBobComponents
{
    public class BigBob : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;

        private Collider _collider;

        private void Awake() => _collider = GetComponent<Collider>();

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
            Health -= damage;
            OnHit?.Invoke(hitPoint, knockBack);
            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player)
        {
        }

        public override void SetIsAttacking(bool isAttacking)
        {
            base.SetIsAttacking(isAttacking);
            _collider.isTrigger = isAttacking;
        }

        private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position + Vector3.up * YOffset, AoeRadius);
    }
}
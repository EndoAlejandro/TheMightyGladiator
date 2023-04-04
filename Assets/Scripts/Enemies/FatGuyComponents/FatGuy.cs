using System;
using PlayerComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.FatGuyComponents
{
    public class FatGuy : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;
        public event Action OnAttackCollision;

        [SerializeField] private float aoeTime = 2f;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private int bulletsAmount = 10;

        public float AoETime => aoeTime;
        public float NormalizedHealth => Health / MaxHealth;
        public Bullet BulletPrefab => bulletPrefab;
        public int BulletsAmount => bulletsAmount;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
            Health -= damage;
            VfxManager.Instance.PlayFloatingText(transform.position + Vector3.up * 2f, damage.ToString(".#"), IsStun);
            OnHit?.Invoke(hitPoint, knockBack);
            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player)
        {
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);

            if (!IsAttacking) return;
            if (collision.transform.CompareTag("Ground")) return;
            if (collision.transform.TryGetComponent(out Enemy enemy)) return;
            OnAttackCollision?.Invoke();
        }
    }
}
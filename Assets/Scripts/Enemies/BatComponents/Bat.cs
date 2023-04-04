using System;
using PlayerComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.BatComponents
{
    public class Bat : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;
        public event Action<Player> OnParry;
        public event Action OnAttackCollision;

        [Header("Health")]
        [SerializeField] private float idleTime = 2f;

        [SerializeField] private float attackSpeed = 10f;
        [SerializeField] private float distanceTolerance = 2f;
        [SerializeField] private float attackTime = 0.5f;
        [SerializeField] private float detectionDistance;

        public float AttackSpeed => attackSpeed;

        public float IdleTime => idleTime;
        public float DistanceTolerance => distanceTolerance;
        public float AttackTime => attackTime;
        public float DetectionDistance => detectionDistance;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0f)
        {
            Health -= damage;
            VfxManager.Instance.PlayFloatingText(transform.position + Vector3.up * 2f, damage.ToString(".#"), IsStun);
            OnHit?.Invoke(hitPoint, knockBack);

            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player) => OnParry?.Invoke(player);

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
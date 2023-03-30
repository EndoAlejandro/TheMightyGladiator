using System;
using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class Bat : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;
        public event Action<Player> OnParry;

        [Header("Health")]
        [SerializeField] private Transform sphere;

        [Header("Attack and Defend")]
        [SerializeField] private float idleTime = 2f;

        [SerializeField] private float deathTime = 1f;
        [SerializeField] private float attackSpeed = 10f;

        public float AttackSpeed => attackSpeed;
        public float DeathTime => deathTime;

        private Rigidbody _rigidbody;
        private float _health;
        public override bool IsAlive => _health > 0;
        public float IdleTime => idleTime;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void OnEnable() => _health = maxHealth;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0f)
        {
            _health -= damage;
            OnHit?.Invoke(hitPoint, knockBack);

            if (_health <= 0)
            {
                OnDead?.Invoke(this);
                ReturnToPool();
            }
        }

        public override void Parry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, player.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            OnParry?.Invoke(player);
        }
    }
}
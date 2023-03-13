using System;
using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class Bat : Enemy
    {
        public event Action<Player> OnHit;
        public event Action<Player> OnKnockBack;
        public event Action<Player> OnParry;

        [Header("Health")]
        [SerializeField] private Transform sphere;
        [SerializeField] private float maxHealth;

        [Header("Movement")]
        [SerializeField] private float speed = 2f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float rotationSpeed = 100f;

        [Header("Attack and Defend")]

        [SerializeField] private float stunTime = 1f;
        [SerializeField] private float deathTime = 1f;
        [SerializeField] private float attackSpeed = 10f;

        public bool IsOnKnockBack { get; private set; }
        public float Speed => speed;
        public float Acceleration => acceleration;
        public float RotationSpeed => rotationSpeed;
        public float StoppingDistance => stoppingDistance;
        public float StunTime => stunTime;
        public float AttackSpeed => attackSpeed;
        public float DeathTime => deathTime;

        private Rigidbody _rigidbody;
        private float _health;
        public override bool IsAlive => _health > 0;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _health = maxHealth;
        }

        public override void TakeDamage(Player player)
        {
            OnHit?.Invoke(player);
            _health--;
        }

        public void KnockBack(Player player) => OnKnockBack?.Invoke(player);

        public override void Parry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, player.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            OnParry?.Invoke(player);
        }

        
        public void SetOnKnockBack(bool isOnKnockBack) => IsOnKnockBack = isOnKnockBack;
    }
}
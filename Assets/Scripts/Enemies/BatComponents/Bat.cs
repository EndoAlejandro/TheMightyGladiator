using System;
using System.Collections;
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
        [SerializeField] private float prepareToAttackTime = 1f;

        [SerializeField] private float stunTime = 1f;
        [SerializeField] private float deathTime = 1f;
        [SerializeField] private float attackSpeed = 10f;

        public bool IsAttacking { get; private set; }
        public bool IsOnKnockBack { get; private set; }
        public float Speed => speed;
        public float Acceleration => acceleration;
        public float RotationSpeed => rotationSpeed;
        public float StoppingDistance => stoppingDistance;
        public float StunTime => stunTime;
        public float PrepareToAttackTime => prepareToAttackTime;
        public float AttackSpeed => attackSpeed;
        public float DeathTime => deathTime;
        public bool IsAlive => _health > 0f;

        private Rigidbody _rigidbody;
        private float _health;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _health = maxHealth;
        }

        public void GetHit(Player player)
        {
            OnHit?.Invoke(player);
            _health--;
        }

        public void KnockBack(Player player) => OnKnockBack?.Invoke(player);

        public void Parry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, player.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            OnParry?.Invoke(player);
        }

        public void SetIsAttacking(bool isAttacking) => IsAttacking = isAttacking;
        public void SetOnKnockBack(bool isOnKnockBack) => IsOnKnockBack = isOnKnockBack;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(sphere.position, sphere.position + transform.forward);
        }
    }
}
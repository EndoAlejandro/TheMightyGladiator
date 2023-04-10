using System;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class Bat : Enemy
    {
        public event Action OnAttackCollision;

        [Header("Health")]

        [SerializeField] private float attackSpeed = 10f;
        [SerializeField] private float attackTime = 0.5f;
        
        public float AttackSpeed => attackSpeed;
        public float AttackTime => attackTime;

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
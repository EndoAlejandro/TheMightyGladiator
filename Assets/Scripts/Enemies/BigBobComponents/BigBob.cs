using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBob : Enemy
    {
        public override event Action<Enemy> OnDead;
        
        [SerializeField] private BigBobBullet bullet;
        [SerializeField] private int bulletsAmount = 5;
        [SerializeField] private float attackRange;
        [SerializeField] private float yOffset = 0.5f;

        public BigBobBullet Bullet => bullet;

        public int BulletsAmount => bulletsAmount;
        public override bool IsAlive => true;
        public float AttackRange => attackRange;
        public float YOffset => yOffset;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public override void TakeDamage(Vector3 position)
        {
        }

        public override void Parry(Player player)
        {
        }

        public override void SetIsAttacking(bool isAttacking)
        {
            base.SetIsAttacking(isAttacking);
            _collider.isTrigger = isAttacking;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * YOffset, attackRange);
        }
    }
}
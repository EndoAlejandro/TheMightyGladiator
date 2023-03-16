using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBob : Enemy
    {
        [SerializeField] private BigBobBullet bullet;
        [SerializeField] private int bulletsAmount = 5;

        public BigBobBullet Bullet => bullet;

        public int BulletsAmount => bulletsAmount;
        public override bool IsAlive => true;

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
    }
}
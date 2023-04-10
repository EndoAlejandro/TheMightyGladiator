using System;
using UnityEngine;

namespace Enemies.FatGuyComponents
{
    public class FatGuy : Enemy
    {
        public event Action OnAttackCollision;

        [SerializeField] private int bulletsAmount = 10;

        public float NormalizedHealth => Health / MaxHealth;
        public int BulletsAmount => bulletsAmount;

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
using System;
using PlayerComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.FatGuyComponents
{
    public class FatGuy : Enemy
    {
        public event Action OnAttackCollision;

        [SerializeField] private float aoeTime = 2f;
        [SerializeField] private int bulletsAmount = 10;

        public float AoETime => aoeTime;
        public float NormalizedHealth => Health / MaxHealth;
        public int BulletsAmount => bulletsAmount;


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
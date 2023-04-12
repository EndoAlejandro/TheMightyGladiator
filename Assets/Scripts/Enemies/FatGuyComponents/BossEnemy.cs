using System;
using UnityEngine;
using Upgrades;

namespace Enemies.FatGuyComponents
{
    public class BossEnemy : Enemy
    {
        public event Action OnAttackCollision;

        [SerializeField] private Upgrade[] upgradePrefabs;
        [SerializeField] private Upgrade healPrefab;
        [SerializeField] private int bulletsAmount = 10;

        public float NormalizedHealth => Health / MaxHealth;
        public int BulletsAmount => bulletsAmount;
        public Upgrade[] UpgradePrefabs => upgradePrefabs;
        public Upgrade HealPrefab => healPrefab;

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
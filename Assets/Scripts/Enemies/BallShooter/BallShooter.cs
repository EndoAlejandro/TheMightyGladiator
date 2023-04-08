using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BallShooter
{
    public class BallShooter : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;

        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private int bulletsPerRound = 6;
        [SerializeField] private int roundsAmount = 4;
        [SerializeField] private float shootRate = 1f;
        [SerializeField] private float bulletSpeed = 2f;
        [Range(0f, 360f)] [SerializeField] private float shootingAngle = 180f;

        public Bullet BulletPrefab => bulletPrefab;
        public int BulletsPerRound => bulletsPerRound;
        public int RoundsAmount => roundsAmount;
        public float ShootRate => shootRate;
        public float ShootingAngle => shootingAngle;
        public float BulletSpeed => bulletSpeed;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
            Health -= damage;
            OnHit?.Invoke(hitPoint, knockBack);
            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player)
        {
        }
    }
}
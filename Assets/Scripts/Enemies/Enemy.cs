using System;
using PlayerComponents;
using Pooling;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : PooledMonoBehaviour, IDealDamage
    {
        public abstract event Action<Enemy> OnDead;
        public event Action<Enemy> OnDeSpawn;
        public abstract event Action<Vector3, float> OnHit;

        [Header("Base Movement")]
        [SerializeField] protected float maxHealth;

        [SerializeField] private float speed = 2f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float rotationSpeed = 100f;

        [Header("Base Attack")]
        [SerializeField] private int damage = 1;

        [SerializeField] private float parryTimeWindow = 0.5f;
        [SerializeField] private float telegraphTime = 1f;
        [SerializeField] private float recoverTime = 1f;

        [Header("Base Get Hit")]
        [SerializeField] private float stunTime = 1f;

        [SerializeField] private float getHitTime = 1f;
        [SerializeField] private float deathTime = 1f;

        public float ParryTimeWindow => parryTimeWindow;
        public float TelegraphTime => telegraphTime;
        public float Speed => speed;
        public float Acceleration => acceleration;
        public float StoppingDistance => stoppingDistance;
        public float RotationSpeed => rotationSpeed;
        public bool IsAttacking { get; private set; }
        public bool CanBeParried { get; private set; }
        public bool IsStun { get; private set; }
        public abstract bool IsAlive { get; }
        public float StunTime => stunTime;
        public float GetHitTime => getHitTime;
        public float RecoverTime => recoverTime;
        public float DeathTime => deathTime;
        public int Damage => damage;

        public abstract void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0f);
        public abstract void Parry(Player player);
        public virtual void SetIsAttacking(bool isAttacking) => IsAttacking = isAttacking;
        public void SetCanBeParried(bool canBeParried) => CanBeParried = canBeParried;
        public void SetIsStun(bool isStun) => IsStun = isStun;

        public void DeSpawn()
        {
            OnDeSpawn?.Invoke(this);
            ReturnToPool();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent(out Player player))
                player.TryToGetDamageFromEnemy(this);
        }
    }
}
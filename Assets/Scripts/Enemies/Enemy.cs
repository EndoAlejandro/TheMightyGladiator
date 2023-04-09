using System;
using PlayerComponents;
using Pooling;
using Rooms;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : PooledMonoBehaviour, IDealDamage
    {
        public abstract event Action<Enemy> OnDead;
        public event Action<Enemy> OnDeSpawn;
        public event Action OnPlayerOnRange;
        public abstract event Action<Vector3, float> OnHit;

        [Header("Base Movement")]
        [SerializeField] private LayerMask ignoreGroundLayerMask;

        [SerializeField] protected float maxHealth;

        [SerializeField] private float speed = 2f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float attackRotationSpeed = 5f;

        [SerializeField] private float detectionDistance = 5f;

        [Header("Shared Attack")]
        [SerializeField] private int damage = 1;

        [SerializeField] private float parryTimeWindow = 0.5f;
        [SerializeField] private float telegraphTime = 1f;
        [SerializeField] private float recoverTime = 1f;

        [Header("Melee Attack")]
        [SerializeField] private float chaseTime = 5f;

        [SerializeField] private float aoeRadius = 1f;

        [Header("Ranged Attack")]
        [SerializeField] private Bullet bulletPrefab;

        [SerializeField] private MortarBomb mortarPrefab;
        [SerializeField] private float accuracy = 1f;
        [Range(0f, 360f)] [SerializeField] private float shootingAngle = 180f;
        [SerializeField] private float bulletSpeed = 2f;
        [SerializeField] private float shootRate = 1f;
        [SerializeField] private int bulletsPerRound = 6;
        [SerializeField] private int roundsAmount = 4;

        [Header("Base Get Hit")]
        [SerializeField] private float stunTime = 1f;

        [SerializeField] private float getHitTime = 1f;
        [SerializeField] private float deathTime = 1f;

        public BaseRoom Room { get; private set; }
        public Bullet BulletPrefab => bulletPrefab;
        public MortarBomb MortarPrefab => mortarPrefab;

        public float ParryTimeWindow => parryTimeWindow;
        public float MaxHealth => maxHealth;
        public float TelegraphTime => telegraphTime;
        public float Speed => speed;
        public float BulletSpeed => bulletSpeed;
        public float ShootRate => shootRate;
        public int BulletsPerRound => bulletsPerRound;
        public int RoundsAmount => roundsAmount;
        public float Acceleration => acceleration;
        public float StoppingDistance => stoppingDistance;
        public float RotationSpeed => rotationSpeed;
        public float AttackRotationSpeed => attackRotationSpeed;
        public float StunTime => stunTime;
        public float GetHitTime => getHitTime;
        public float RecoverTime => recoverTime;
        public float ShootingAngle => shootingAngle;
        public float DeathTime => deathTime;
        public float DetectionDistance => detectionDistance;
        public float AoeRadius => aoeRadius;
        public float Accuracy => accuracy;
        public float Health { get; protected set; }
        public bool IsAttacking { get; private set; }
        public bool CanBeParried { get; private set; }
        public bool IsStun { get; private set; }
        public bool PlayerDetected { get; private set; }
        public bool IsAlive => Health > 0f;
        public int Damage => damage;
        public LayerMask IgnoreGroundLayerMask => ignoreGroundLayerMask;
        public float ChaseTime => chaseTime;

        public abstract void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0f);
        public abstract void Parry(Player player);
        public virtual void SetIsAttacking(bool isAttacking) => IsAttacking = isAttacking;
        public void SetCanBeParried(bool canBeParried) => CanBeParried = canBeParried;
        public void SetIsStun(bool isStun) => IsStun = isStun;
        public void Setup(BaseRoom room) => Room = room;

        protected virtual void OnEnable()
        {
            PlayerDetected = false;
            Health = MaxHealth;
        }

        public virtual void PlayerOnRange()
        {
            PlayerDetected = true;
            OnPlayerOnRange?.Invoke();
        }

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, StoppingDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, DetectionDistance);
        }
    }
}
using System;
using FxComponents;
using PlayerComponents;
using Pooling;
using UnityEngine;

namespace Enemies.Combat
{
    /// <summary>
    /// Slim, clean enemy core: health, damage in/out, death, pooling and the combat
    /// state flags. It deliberately knows nothing about movement or how the enemy
    /// attacks — attack behaviour is a single <see cref="PerformAttack"/> hook that
    /// <see cref="MeleeEnemy"/> / <see cref="RangedEnemy"/> implement.
    /// </summary>
    public abstract class CombatEnemy : PooledMonoBehaviour, IDealDamage, IDamageable
    {
        public event Action<CombatEnemy> OnDead;
        public event Action<CombatEnemy> OnDeSpawn;
        public event Action<Vector3, float> OnHit;
        public event Action<Player> OnParry;

        [Header("Health")]
        [SerializeField] private float _maxHealth = 20f;

        [Header("Combat")]
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _parryTimeWindow = 0.5f;
        [Tooltip("If true the enemy hurts the player just by touching it. Dancing enemies usually want this OFF.")]
        [SerializeField] private bool _dealsContactDamage;

        private Rigidbody _rigidbody;

        public float MaxHealth => _maxHealth;
        public float Health { get; private set; }
        public bool IsAlive => Health > 0f;
        public int Damage => _damage;
        public float ParryTimeWindow => _parryTimeWindow;
        public Vector3 Velocity => _rigidbody == null ? Vector3.zero : _rigidbody.linearVelocity;

        public bool IsAttacking { get; private set; }
        public bool CanBeParried { get; private set; }
        public bool IsStun { get; private set; }

        protected virtual void Awake() => _rigidbody = GetComponent<Rigidbody>();

        protected virtual void OnEnable()
        {
            Health = _maxHealth;
            IsAttacking = false;
            CanBeParried = false;
            IsStun = false;
        }

        public void TakeDamage(Vector3 hitPoint, float incomingDamage, float knockBack = 0f)
        {
            if (!IsAlive) return;

            Health -= incomingDamage;
            OnHit?.Invoke(hitPoint, knockBack);
            VfxManager.Instance.PlayFloatingText(transform.position + Vector3.up * 2f,
                Mathf.RoundToInt(incomingDamage).ToString(), IsStun);
            SfxManager.Instance.PlayFx(Sfx.EnemyHit, transform.position);

            if (!IsAlive) OnDead?.Invoke(this);
        }

        public void Parry(Player player) => OnParry?.Invoke(player);
        public void SetIsAttacking(bool value) => IsAttacking = value;
        public void SetCanBeParried(bool value) => CanBeParried = value;
        public void SetIsStun(bool value) => IsStun = value;

        public void DeSpawn()
        {
            OnDeSpawn?.Invoke(this);
            ReturnToPool();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!_dealsContactDamage) return;
            if (collision.transform.TryGetComponent(out Player player))
                player.TryToGetDamageFromEnemy(this);
        }

        /// <summary>Run the actual attack. Called from an attack state once you wire it up.</summary>
        public abstract void PerformAttack();
    }
}

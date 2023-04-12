using System;
using System.Collections;
using CustomUtils;
using FxComponents;
using JetBrains.Annotations;
using UnityEngine;
using VfxComponents;

namespace PlayerComponents
{
    public class Player : Singleton<Player>
    {
        public event Action OnHit;
        public event Action OnParry;
        public event Action OnShieldHit;
        public event Action OnUpgrade;
        public event Action OnDead;
        public event Action OnTeleport;

        [SerializeField] private PlayerData currentPlayerData;
        [SerializeField] private LayerMask attackLayerMask;
        [Range(5f, 90f)] [SerializeField] private float attackAngle = 45f;
        [SerializeField] private float hitBoxSize = 1.5f;
        [Range(5f, 270f)] [SerializeField] private float defendAngle = 90f;
        [SerializeField] private float defendBoxSize = 1f;

        private float _attackTimer;
        private float _dodgeTimer;
        private float _defendTimer;
        private float _immunityTimer;
        private bool _shieldActive;

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public float WalkSpeed { get; private set; }
        public float AttackRate { get; private set; }
        public float Acceleration { get; private set; }
        public float RotationSpeed { get; private set; }
        public float KnockBackForce { get; private set; }
        public float DodgeRate { get; private set; }
        public float Damage { get; private set; }
        public float DefendRate { get; private set; }
        public float ImmunityTime { get; private set; }
        public float HitBoxSize => hitBoxSize;
        public float DefendBoxSize => defendBoxSize;
        public float AttackAngle => attackAngle;
        public float DefendAngle => defendAngle;
        public float CriticalProbability { get; private set; }
        public float ParryTime { get; private set; }
        public float Height { get; private set; }
        public float DodgeSpeed { get; private set; }
        public float DodgeDistance { get; private set; }
        public float CriticalDamage { get; private set; }
        public bool IsDodging { get; private set; }
        public bool CanMove { get; private set; } = true;
        public bool CanBeHealed => Health < MaxHealth;
        public bool CanAttack => _attackTimer <= 0f;
        public bool CanDodge => _dodgeTimer <= 0f;
        public bool CanDefend => _defendTimer <= 0f;
        public bool IsImmune => _immunityTimer > 0f;
        public bool IsAlive => Health > 0f;
        public LayerMask AttackLayerMask => attackLayerMask;

        private Collider _collider;
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            Height = _collider.bounds.center.y;
            SetPlayerData(currentPlayerData);
        }

        private void Update()
        {
            if (!CanAttack) _attackTimer -= Time.deltaTime;
            if (!CanDodge) _dodgeTimer -= Time.deltaTime;
            if (!CanDefend) _defendTimer -= Time.deltaTime;
            if (_immunityTimer > 0f) _immunityTimer -= Time.deltaTime;
        }

        public void Attack() => _attackTimer = AttackRate;

        public void SetDodgeState(bool state)
        {
            IsDodging = state;
            if (!state) _dodgeTimer = DodgeRate;
        }

        public void Parry() => OnParry?.Invoke();

        private void ShieldHit()
        {
            SfxManager.Instance.PlayFx(Sfx.ShieldHit, transform.position);
            VfxManager.Instance.PlayFx(Vfx.PlayerHit, transform.position + Vector3.up);
            OnShieldHit?.Invoke();
        }

        private void TakeDamage(Vector3 direction, int damageAmount)
        {
            Health -= damageAmount;

            SfxManager.Instance.PlayFx(Sfx.PlayerHit, transform.position);
            VfxManager.Instance.PlayFx(Vfx.PlayerHit, transform.position + Vector3.up);
            _immunityTimer = ImmunityTime;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(direction * 15f, ForceMode.VelocityChange);
            OnHit?.Invoke();

            if (!(Health <= 0f)) return;

            OnDead?.Invoke();
            VfxManager.Instance.PlayFloatingText(transform.position + Vector3.up, damageAmount.ToString(), false);
            VfxManager.Instance.PlayFx(Vfx.SwordCritical, transform.position + Vector3.up);
        }

        public void SetShieldActive(bool value) => _shieldActive = value;

        public bool TryToGetDamageFromEnemy(IDealDamage damageDealer, bool ignoreShield = false)
        {
            if (IsDodging) return false;
            if (IsImmune) return false;

            Vector3 direction;
            if (damageDealer.transform.TryGetComponent(out Bullet bullet))
                direction = -damageDealer.Velocity.normalized;
            else
                direction = Utils.NormalizedFlatDirection(damageDealer.transform.position, transform.position);

            if (_shieldActive && !ignoreShield)
            {
                if (Vector3.Angle(direction, transform.forward) < DefendAngle)
                {
                    ShieldHit();
                    return false;
                }
            }

            TakeDamage(-direction, damageDealer.Damage);
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + (Vector3.up * Height), defendBoxSize);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3.up * Height), hitBoxSize);
        }

        public void Teleport(float time) => StartCoroutine(SleepUntilTimeEnded(time));

        private IEnumerator SleepUntilTimeEnded(float time)
        {
            CanMove = false;
            OnTeleport?.Invoke();
            yield return new WaitForSecondsRealtime(time);
            CanMove = true;
        }

        public void IncreaseDamage(float value)
        {
            Damage += value;
            OnUpgrade?.Invoke();
        }

        public void IncreaseCriticalProbability(float value)
        {
            CriticalProbability += value;
            OnUpgrade?.Invoke();
        }

        public void IncreaseCriticalDamage(float value)
        {
            CriticalDamage += value;
            OnUpgrade?.Invoke();
        }

        public void IncreaseMaxHealth(int value)
        {
            MaxHealth += value;
            OnUpgrade?.Invoke();
        }

        public void Heal(int value)
        {
            Health = Mathf.Min(Health + value, MaxHealth);
            OnUpgrade?.Invoke();
        }

        public void SetPlayerData(PlayerData playerData)
        {
            MaxHealth = playerData.MaxHealth;
            Health = playerData.Health;
            ImmunityTime = playerData.ImmunityTime;
            WalkSpeed = playerData.WalkSpeed;
            Acceleration = playerData.Acceleration;
            RotationSpeed = playerData.RotationSpeed;
            DodgeRate = playerData.DodgeRate;
            DodgeSpeed = playerData.DodgeSpeed;
            DodgeDistance = playerData.DodgeDistance;
            Damage = playerData.Damage;
            CriticalProbability = playerData.CriticalProbability;
            CriticalDamage = playerData.CriticalDamage;
            AttackRate = playerData.AttackRate;
            DefendRate = playerData.DefendRate;
            ParryTime = playerData.ParryTime;
            KnockBackForce = playerData.KnockBackForce;
            SaveData();
        }

        public void SaveData() => currentPlayerData.SaveData(this);
    }
}
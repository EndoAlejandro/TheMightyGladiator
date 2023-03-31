using System;
using System.Collections;
using CustomUtils;
using Enemies;
using UnityEngine;

namespace PlayerComponents
{
    public class Player : MonoBehaviour
    {
        public event Action<Vector3> OnDealDamage;
        public event Action OnHit;
        public event Action OnParry;
        public event Action OnShieldHit;

        [Header("Health")]
        [SerializeField] private int maxHealth;

        [SerializeField] private float immunityTime = 1.5f;

        [Header("Locomotion")]
        [SerializeField] private float walkSpeed = 6f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float rotationSpeed = 100f;

        [Header("Dodge")]
        [SerializeField] private float dodgeRate = 0.25f;

        [SerializeField] private float dodgeSpeed = 12f;
        [SerializeField] private float dodgeDistance = 3f;

        [Header("Attack")]
        [SerializeField] private float damage = 1f;

        [Range(0f, 1f)]
        [SerializeField] private float criticalProbability;

        [Range(1f, 2f)]
        [SerializeField] private float criticalDamage = 2f;

        [SerializeField] private LayerMask attackLayerMask;
        [Range(5f, 90f)] [SerializeField] private float attackAngle = 45f;
        [SerializeField] private float hitBoxSize = 1.5f;
        [SerializeField] private float attackRate = 0.25f;
        [SerializeField] private float knockBackForce = 10f;

        [Header("Defend")]
        [Range(5f, 90f)] [SerializeField] private float defendAngle = 90f;

        [SerializeField] private float defendBoxSize = 1f;
        [SerializeField] private float parryTime = 0.25f;
        [SerializeField] private float defendRate = 0.5f;

        public int Health { get; private set; }

        private float _attackTimer;
        private float _dodgeTimer;
        private float _defendTimer;
        private float _immunityTimer;

        private bool _shieldActive;

        public bool CanAttack => _attackTimer <= 0f;
        public bool CanDodge => _dodgeTimer <= 0f;
        public bool CanDefend => _defendTimer <= 0f;
        public bool IsImmune => _immunityTimer > 0f;
        public bool IsDodging { get; private set; }
        public float WalkSpeed => walkSpeed;
        public float Acceleration => acceleration;
        public float RotationSpeed => rotationSpeed;
        public float KnockBackForce => knockBackForce;
        public float Damage => damage;
        public float HitBoxSize => hitBoxSize;
        public float DefendBoxSize => defendBoxSize;
        public float AttackAngle => attackAngle;
        public float DefendAngle => defendAngle;
        public float ParryTime => parryTime;
        public LayerMask AttackLayerMask => attackLayerMask;
        public float Height { get; private set; }
        public float DodgeSpeed => dodgeSpeed;
        public float DodgeDistance => dodgeDistance;
        public bool CanMove { get; private set; } = true;
        public float CriticalDamage => criticalDamage;

        private Collider _collider;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            Height = _collider.bounds.center.y;
        }

        private void Start() => Health = maxHealth;

        private void Update()
        {
            if (!CanAttack) _attackTimer -= Time.deltaTime;
            if (!CanDodge) _dodgeTimer -= Time.deltaTime;
            if (!CanDefend) _defendTimer -= Time.deltaTime;
            if (_immunityTimer > 0f) _immunityTimer -= Time.deltaTime;
        }

        public void Attack() => _attackTimer = attackRate;

        public void SetDodgeState(bool state)
        {
            IsDodging = state;
            if (!state) _dodgeTimer = dodgeRate;
        }

        public void Parry()
        {
            _defendTimer = defendRate;
            _immunityTimer = immunityTime;
            OnParry?.Invoke();
        }

        public void ShieldHit() => OnShieldHit?.Invoke();

        private void TakeDamage(Vector3 direction, int amount)
        {
            _immunityTimer = immunityTime;
            _rigidbody.AddForce(direction * 15f, ForceMode.VelocityChange);
            OnHit?.Invoke();
        }

        public void SetShieldActive(bool value) => _shieldActive = value;

        public bool TryToGetDamageFromEnemy(IDealDamage damageDealer)
        {
            if (IsDodging) return false;
            if (IsImmune) return false;
            //if (!_shieldActive) return true;

            var direction = Utils.NormalizedFlatDirection(damageDealer.transform.position, transform.position);

            if (_shieldActive)
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

        public void Sleep(float time) => StartCoroutine(SleepUntilTimeEnded(time));

        private IEnumerator SleepUntilTimeEnded(float time)
        {
            CanMove = false;
            yield return new WaitForSecondsRealtime(time);
            CanMove = true;
        }

        public void Teleport()
        {
            //TODO: Teleport event and stop moving from input.
        }

        public void IncreaseDamage(float value) => damage += value;
        public void IncreaseCriticalProbability(float value) => criticalProbability += value;
        public void IncreaseCriticalDamage(float value) => criticalDamage += value;
        public void IncreaseMaxHealth(int value) => maxHealth += value;
        public void Heal(int value) => Health += value;
    }
}
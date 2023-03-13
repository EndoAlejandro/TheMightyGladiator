using System;
using CustomUtils;
using Enemies;
using Enemies.BatComponents;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerComponents
{
    public class Player : MonoBehaviour
    {
        public event Action<Vector3> OnDealDamage;
        public event Action OnHit;
        public event Action OnParry;
        public event Action OnShieldHit;

        [Header("Locomotion")]
        [SerializeField] private float speed = 3f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float rotationSpeed = 100f;

        [SerializeField] private float dodgeCd = 0.25f;

        [Header("Attack and Defend")]
        [SerializeField] private LayerMask attackLayerMask;

        [Range(5f, 90f)] [SerializeField] private float attackAngle = 45f;
        [SerializeField] private float hitBoxSize = 1.5f;
        [SerializeField] private float attackRate;
        [SerializeField] private float force;
        [Range(5f, 90f)] [SerializeField] private float defendAngle = 90f;
        [SerializeField] private float defendBoxSize = 1f;
        [SerializeField] private float parryTime;
        [SerializeField] private float defendRate = 0.5f;

        [Header("Health")]
        [SerializeField] private float immunityTime = 1.5f;

        private float _attackTimer;
        private float _dodgeTimer;
        private float _defendTimer;
        private float _immunityTimer;

        private bool _shieldActive;

        public bool CanAttack => _attackTimer <= 0f;
        public bool CanDodge => _dodgeTimer <= 0f;
        public bool CanDefend => _defendTimer <= 0f;
        public bool IsImmune => _immunityTimer > 0f;
        public float Speed => speed;
        public float Acceleration => acceleration;
        public float RotationSpeed => rotationSpeed;
        public float Force => force;
        public float HitBoxSize => hitBoxSize;
        public float DefendBoxSize => defendBoxSize;
        public float AttackAngle => attackAngle;
        public float DefendAngle => defendAngle;
        public float ParryTime => parryTime;
        public LayerMask AttackLayerMask => attackLayerMask;
        public float Height { get; private set; }

        private Collider _collider;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            Height = _collider.bounds.center.y;
        }

        private void Update()
        {
            if (!CanAttack) _attackTimer -= Time.deltaTime;
            if (!CanDodge) _dodgeTimer -= Time.deltaTime;
            if (!CanDefend) _defendTimer -= Time.deltaTime;
            if (_immunityTimer > 0f) _immunityTimer -= Time.deltaTime;
        }

        public void Attack() => _attackTimer = attackRate;

        public void Dodge() => _dodgeTimer = dodgeCd;

        public void Parry()
        {
            _defendTimer = defendRate;
            _immunityTimer = immunityTime;
            Debug.Log("Parry!");
            OnParry?.Invoke();
        }

        public void Hit() => OnHit?.Invoke();

        public void ShieldHit() => OnShieldHit?.Invoke();

        public void TakeDamage(Enemy enemy)
        {
            _immunityTimer = immunityTime;
            var direction = Utils.NormalizedFlatDirection(transform.position, enemy.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            Hit();
        }

        public void DealDamage(Vector3 hitPoint) => OnDealDamage?.Invoke(hitPoint + Vector3.up * Height);

        public void SetShieldActive(bool value) => _shieldActive = value;

        public bool TryToDealDamage(Vector3 enemyPosition)
        {
            if (IsImmune) return false;
            if (!_shieldActive) return true;

            var direction = Utils.NormalizedFlatDirection(enemyPosition, transform.position);
            var angle = Vector3.Angle(direction, transform.forward);
            return angle > DefendAngle;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + (Vector3.up * Height), defendBoxSize);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3.up * Height), hitBoxSize);
        }
    }
}
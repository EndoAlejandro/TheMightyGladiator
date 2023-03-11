using System;
using UnityEngine;

namespace PlayerComponents
{
    public class Player : MonoBehaviour
    {
        [Header("Locomotion")]
        [SerializeField] private float speed = 3f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float rotationSpeed = 100f;

        [SerializeField] private float dodgeCd = 0.25f;

        [Header("Attack and Defend")]
        [SerializeField] private float attackRate;

        [SerializeField] private float force;

        private float _attackTimer;
        private float _dodgeTimer;

        public bool CanAttack => _attackTimer <= 0f;
        public bool CanDodge => _dodgeTimer <= 0f;

        public float Speed => speed;
        public float Acceleration => acceleration;
        public float RotationSpeed => rotationSpeed;
        public float Force => force;
        public float Height { get; private set; }

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Height = _collider.bounds.center.y;
        }

        private void Update()
        {
            if (!CanAttack) _attackTimer -= Time.deltaTime;
            if (!CanDodge) _dodgeTimer -= Time.deltaTime;
        }

        public void Attack() => _attackTimer = attackRate;
        public void Dodge() => _dodgeTimer = dodgeCd;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + transform.forward + (Vector3.up * Height), Vector3.one);
        }
    }
}
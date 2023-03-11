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

        [Header("Attack and Defend")]
        [SerializeField] private float attackRate;

        [SerializeField] private float damage;

        private float _attackTimer;

        public bool CanAttack => _attackTimer <= 0;

        public float Speed => speed;
        public float Acceleration => acceleration;
        public float RotationSpeed => rotationSpeed;
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
        }

        public void Attack() => _attackTimer = attackRate;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + transform.forward + (Vector3.up * Height), Vector3.one);
        }
    }
}
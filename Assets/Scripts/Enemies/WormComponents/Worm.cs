using System;
using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies.WormComponents
{
    public class Worm : Enemy
    {
        public event Action<Player> OnParry;

        [SerializeField] private float idleTime;
        [SerializeField] private float attackRadius = 0.65f;

        private Collider _collider;
        private Rigidbody _rigidbody;
        public override bool IsAlive => true;

        public float IdleTime => idleTime;
        public float AttackRadius => attackRadius;
        public float Height { get; private set; }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            Height = _collider.bounds.center.y;
        }

        private void Start() => SetIsAttacking(false);

        public override void TakeDamage(Player player)
        {
        }

        public void SetColliderState(bool state)
        {
            if (_collider == null) _collider = GetComponent<Collider>();
            _collider.enabled = state;
        }

        public override void Parry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, player.transform.position);
            _rigidbody.AddForce(direction * 10f, ForceMode.VelocityChange);
            OnParry?.Invoke(player);
        }
    }
}
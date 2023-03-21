using System;
using PlayerComponents;
using Pooling;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : PooledMonoBehaviour
    {
        public abstract event Action<Enemy> OnDead;
        
        [Header("Base Movement")]
        [SerializeField] protected float maxHealth;

        [SerializeField] private float speed = 2f;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float rotationSpeed = 100f;

        [Header("Base Attack")]
        [SerializeField] private float parryTimeWindow = 0.5f;

        [SerializeField] private float prepareToAttackTime = 1f;
        public float ParryTimeWindow => parryTimeWindow;
        public float PrepareToAttackTime => prepareToAttackTime;
        public float Speed => speed;
        public float Acceleration => acceleration;
        public float StoppingDistance => stoppingDistance;
        public float RotationSpeed => rotationSpeed;
        public bool IsAttacking { get; private set; }
        public bool CanBeParried { get; private set; }

        public abstract bool IsAlive { get; }

        public abstract void TakeDamage(Vector3 position);
        public abstract void Parry(Player player);
        public virtual void SetIsAttacking(bool isAttacking) => IsAttacking = isAttacking;
        public void SetCanBeParried(bool canBeParried) => CanBeParried = canBeParried;
    }
}
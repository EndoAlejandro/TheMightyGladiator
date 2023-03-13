using PlayerComponents;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private float prepareToAttackTime = 1f;
        public float PrepareToAttackTime => prepareToAttackTime;

        public abstract bool IsAlive { get; }
        public bool IsAttacking { get; protected set; }

        public abstract void TakeDamage(Player player);

        public abstract void Parry(Player player);
        public virtual void SetIsAttacking(bool isAttacking)
        {
            IsAttacking = isAttacking;
        }
    }
}
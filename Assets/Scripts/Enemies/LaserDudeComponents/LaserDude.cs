using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.LaserDudeComponents
{
    public class LaserDude : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
            Health -= Damage;
            OnHit?.Invoke(hitPoint, knockBack);
            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player)
        {
        }
    }
}
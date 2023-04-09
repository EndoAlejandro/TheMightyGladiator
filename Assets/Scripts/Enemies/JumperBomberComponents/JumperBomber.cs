using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.JumperBomberComponents
{
    public class JumperBomber : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
            Health -= damage;
            OnHit?.Invoke(hitPoint, knockBack);
            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player)
        {
        }
    }
}
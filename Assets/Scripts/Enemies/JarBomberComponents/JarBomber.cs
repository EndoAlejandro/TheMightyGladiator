using System;
using PlayerComponents;
using UnityEngine;

namespace Enemies.JarBomberComponents
{
    public class JarBomber : Enemy
    {
        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;

        [SerializeField] private int bombsAmount = 1;
        [SerializeField] private MortarBomb mortarBombPrefab;
        [SerializeField] private float accuracy = 7f;
        public int BombsAmount => bombsAmount;
        public MortarBomb MortarBombPrefab => mortarBombPrefab;
        public float Accuracy => accuracy;

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
        }

        public override void Parry(Player player)
        {
        }
    }
}
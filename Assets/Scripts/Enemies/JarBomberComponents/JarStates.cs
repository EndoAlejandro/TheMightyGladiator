﻿using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies.JarBomberComponents
{
    public class JarBomberAttack : EnemyAttack
    {
        private readonly JarBomber _jarBomber;
        public bool Ended { get; private set; }

        public JarBomberAttack(JarBomber jarBomber) : base(jarBomber) => _jarBomber = jarBomber;

        public override void FixedTick()
        {
            Ended = true;
        }

        public override void OnEnter()
        {
            Ended = false;
            base.OnEnter();

            for (int i = 0; i < _jarBomber.BombsAmount; i++)
            {
                var mortarBullet =
                    _jarBomber.MortarBombPrefab.Get<MortarBomb>(
                        _jarBomber.transform.position.With(y: 1f),
                        Quaternion.identity);
                var target = Player.Instance.transform.position +
                             Random.insideUnitSphere.With(y: 0f).normalized * Random.Range(1f, _jarBomber.Accuracy);
                mortarBullet.Setup(target, 70f);
            }
        }
    }
}
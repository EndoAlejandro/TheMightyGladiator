﻿using CustomUtils;
using FxComponents;
using PlayerComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.AttackStates
{
    public class EnemyMortarShot : EnemyAttack
    {
        private readonly Enemy _enemy;
        private readonly bool _branch;
        private NavMeshHit _navMeshHit;
        public bool Ended { get; private set; }

        public EnemyMortarShot(Enemy enemy, bool branch) : base(enemy)
        {
            _enemy = enemy;
            _branch = branch;
        }

        public override void FixedTick() => Ended = true;

        public override void OnEnter()
        {
            Ended = false;
            base.OnEnter();

            for (int i = 0; i < _enemy.BulletsPerRound; i++)
            {
                var mortarBullet =
                    _enemy.MortarPrefab.Get<MortarBomb>(
                        _enemy.transform.position.With(y: 1f),
                        Quaternion.identity);
                var target = Player.Instance.transform.position +
                             Random.insideUnitSphere.With(y: 0f).normalized * Random.Range(1f, _enemy.Accuracy);

                if (NavMesh.SamplePosition(target, out _navMeshHit, 2f, NavMesh.AllAreas))
                    target = _navMeshHit.position;

                mortarBullet.Setup(target, 70f, _branch);
            }
            SfxManager.Instance.PlayFx(Sfx.MortarShot, _enemy.transform.position);
        }
    }
}
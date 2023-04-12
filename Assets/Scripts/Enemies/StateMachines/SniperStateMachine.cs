using System;
using Enemies.AttackStates;
using Enemies.EnemiesSharedStates;
using Enemies.LaserDudeComponents;
using Enemies.SharedStates;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.SniperComponents
{
    public class SniperStateMachine : FiniteStateBehaviour
    {
        private Enemy _enemy;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private EnemySpawn _spawn;
        private EnemyDeath _death;

        protected override void References()
        {
            _enemy = GetComponent<Enemy>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_enemy, _rigidbody);
            var wormPush = new EnemyWormPush(_enemy, _rigidbody);
            var wormRecover = new EnemyWormRecover(_enemy);
            var telegraph = new EnemyLaserTelegraph(_enemy);
            var sniperShot = new EnemySniperShot(_enemy, _rigidbody);
            var recover = new EnemyRecover(_enemy);
            _death = new EnemyDeath(_enemy, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(patrol, wormRecover, () => _enemy.PlayerDetected);

            stateMachine.AddTransition(wormRecover, wormPush, () => wormRecover.Ended && !wormRecover.PlayerOnRange);
            stateMachine.AddTransition(wormPush, wormRecover, () => wormPush.Ended);

            stateMachine.AddTransition(wormRecover, telegraph,
                () => wormRecover.PlayerOnRange && wormRecover.PlayerInFront);
            stateMachine.AddTransition(telegraph, sniperShot, () => telegraph.Ended);
            stateMachine.AddTransition(sniperShot, recover, () => sniperShot.Ended);
            stateMachine.AddTransition(recover, wormRecover, () => recover.Ended);

            stateMachine.AddTransition(_death, patrol, () => _death.Ended);
        }

        private void EnemyOnDead(Enemy enemy) => stateMachine.SetState(_death);

        private void OnEnable()
        {
            _enemy.OnDead += EnemyOnDead;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable() => _enemy.OnDead -= EnemyOnDead;
    }
}
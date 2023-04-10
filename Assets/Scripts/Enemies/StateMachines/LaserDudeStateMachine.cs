using Enemies.EnemiesSharedStates;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.LaserDudeComponents
{
    public class LaserDudeStateMachine : FiniteStateBehaviour
    {
        private Enemy _enemy;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private EnemySpawn _spawn;
        private EnemyDeath _death;
        private EnemyChaseWalking _chaseWalking;

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
            _chaseWalking = new EnemyChaseWalking(_enemy, _rigidbody);
            var telegraph = new EnemyLaserTelegraph(_enemy);
            var attack = new EnemyLaserAttack(_enemy);
            var recover = new EnemyRecover(_enemy);
            _death = new EnemyDeath(_enemy, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(patrol, _chaseWalking, () => _enemy.PlayerDetected);

            stateMachine.AddTransition(_chaseWalking, telegraph,
                () => _chaseWalking.CanSeePlayer && _chaseWalking.PlayerOnRange && _chaseWalking.PlayerInFront);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _chaseWalking, () => recover.Ended);

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
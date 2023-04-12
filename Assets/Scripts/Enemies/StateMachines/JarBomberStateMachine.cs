using Enemies.AttackStates;
using Enemies.EnemiesSharedStates;
using Enemies.MovementStates;
using Enemies.SharedStates;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.JarBomberComponents
{
    public class JarBomberStateMachine : FiniteStateBehaviour
    {
        private Enemy _enemy;
        private Rigidbody _rigidbody;

        private EnemySpawn _spawn;
        private EnemyChaseWalking _chaseWalking;
        private EnemyDeath _death;
        private Collider _collider;

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
            var telegraph = new EnemyTelegraph(_enemy);
            var attack = new EnemyMortarShot(_enemy, false);
            var recover = new EnemyRecover(_enemy);
            _death = new EnemyDeath(_enemy, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(patrol, _chaseWalking, () => _enemy.PlayerDetected);

            stateMachine.AddTransition(_chaseWalking, telegraph, () => _chaseWalking.PlayerOnRange);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _chaseWalking, () => recover.Ended);

            stateMachine.AddTransition(_death, _chaseWalking, () => _death.Ended);
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
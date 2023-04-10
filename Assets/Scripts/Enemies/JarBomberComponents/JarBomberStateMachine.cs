using System;
using Enemies.EnemiesSharedStates;
using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.JarBomberComponents
{
    public class JarBomberStateMachine : FiniteStateBehaviour
    {
        private JarBomber _jarBomber;
        private Rigidbody _rigidbody;

        private EnemySpawn _spawn;
        private EnemyChaseWalking _chaseWalking;
        private EnemyDeath _death;
        private Collider _collider;

        protected override void References()
        {
            _jarBomber = GetComponent<JarBomber>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_jarBomber, _rigidbody);
            _chaseWalking = new EnemyChaseWalking(_jarBomber, _rigidbody);
            var telegraph = new EnemyTelegraph(_jarBomber);
            var attack = new JarBomberAttack(_jarBomber);
            var recover = new EnemyRecover(_jarBomber);
            _death = new EnemyDeath(_jarBomber, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(patrol, _chaseWalking, () => _jarBomber.PlayerDetected);

            stateMachine.AddTransition(_chaseWalking, telegraph, () => _chaseWalking.PlayerOnRange);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _chaseWalking, () => recover.Ended);

            stateMachine.AddTransition(_death, _chaseWalking, () => _death.Ended);
        }

        private void JarBomberOnDead(Enemy enemy) => stateMachine.SetState(_death);

        private void OnEnable()
        {
            _jarBomber.OnDead += JarBomberOnDead;
            stateMachine.SetState(_spawn);
        }


        private void OnDisable() => _jarBomber.OnDead -= JarBomberOnDead;
    }
}
using System;
using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.JarBomberComponents
{
    public class JarBomberStateMachine : FiniteStateBehaviour
    {
        private JarBomber _jarBomber;
        private Rigidbody _rigidbody;
        private NavigationSteering _navigationSteering;

        private EnemySpawn _spawn;
        private EnemyIdle _idle;

        protected override void References()
        {
            _jarBomber = GetComponent<JarBomber>();
            _rigidbody = GetComponent<Rigidbody>();
            _navigationSteering = GetComponent<NavigationSteering>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_jarBomber, _rigidbody);
            _idle = new EnemyIdle(_jarBomber, _rigidbody, _navigationSteering);
            var telegraph = new EnemyTelegraph(_jarBomber);
            var attack = new JarBomberAttack(_jarBomber);
            var recover = new EnemyRecover(_jarBomber);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(_idle, telegraph, () => _idle.PlayerOnRange);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _idle, () => recover.Ended);
        }

        private void JarBomberOnPlayerOnRange() => stateMachine.SetState(_idle);

        private void OnEnable()
        {
            _jarBomber.OnPlayerOnRange += JarBomberOnPlayerOnRange;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable()
        {
            _jarBomber.OnPlayerOnRange -= JarBomberOnPlayerOnRange;
        }
    }
}
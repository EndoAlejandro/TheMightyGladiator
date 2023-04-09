using System;
using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.JumperBomberComponents
{
    public class JumperBomberStateMachine : FiniteStateBehaviour
    {
        private JumperBomber _jumperBomber;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private NavigationSteering _navigationSteering;

        private EnemySpawn _spawn;
        private EnemyIdle _idle;
        private EnemyDeath _death;

        protected override void References()
        {
            _jumperBomber = GetComponent<JumperBomber>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _navigationSteering = GetComponent<NavigationSteering>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_jumperBomber, _rigidbody);
            _idle = new EnemyIdle(_jumperBomber, _rigidbody, _navigationSteering);
            _death = new EnemyDeath(_jumperBomber, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
        }

        private void JumperBomberOnDead(Enemy enemy) => stateMachine.SetState(_death);
        private void JumperBomberOnPlayerOnRange() => stateMachine.SetState(_idle);

        private void OnEnable()
        {
            _jumperBomber.OnPlayerOnRange += JumperBomberOnPlayerOnRange;
            _jumperBomber.OnDead += JumperBomberOnDead;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable()
        {
            _jumperBomber.OnPlayerOnRange += JumperBomberOnPlayerOnRange;
            _jumperBomber.OnDead += JumperBomberOnDead;
        }
    }
}
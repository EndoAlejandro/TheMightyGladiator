using System;
using Enemies.EnemiesSharedStates;
using Enemies.MovementStates;
using Enemies.SharedStates;
using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.JumperBomberComponents
{
    public class JumperBomberStateMachine : FiniteStateBehaviour
    {
        private Enemy _jumperBomber;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private EnemySpawn _spawn;
        private EnemyChaseWalking _chaseWalking;
        private EnemyDeath _death;
        private EnemyInitialJump _initialJump;

        protected override void References()
        {
            _jumperBomber = GetComponent<Enemy>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_jumperBomber, _rigidbody);
            var telegraph = new EnemyTelegraph(_jumperBomber);
            _initialJump = new EnemyInitialJump(_jumperBomber, _rigidbody, _collider);
            var jumpAir = new EnemyOnAirJump(_jumperBomber);
            var endJump = new EnemyEndJump(_jumperBomber, _rigidbody, _collider);
            var splashAttack = new EnemySplashAttack(_jumperBomber);
            var recover = new EnemyRecover(_jumperBomber);
            _death = new EnemyDeath(_jumperBomber, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(patrol, telegraph, () => _jumperBomber.PlayerDetected);

            stateMachine.AddTransition(telegraph, _initialJump, () => telegraph.Ended);
            stateMachine.AddTransition(_initialJump, jumpAir, () => _initialJump.Ended);
            stateMachine.AddTransition(jumpAir, endJump, () => jumpAir.Ended);
            stateMachine.AddTransition(endJump, splashAttack, () => endJump.Ended);
            stateMachine.AddTransition(splashAttack, recover, ()=> splashAttack.Ended);
            stateMachine.AddTransition(recover, telegraph, () => recover.Ended);

            stateMachine.AddTransition(_death, patrol, () => _death.Ended);
        }

        private void JumperBomberOnDead(Enemy enemy) => stateMachine.SetState(_death);

        private void OnEnable()
        {
            _jumperBomber.OnDead += JumperBomberOnDead;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable() => _jumperBomber.OnDead += JumperBomberOnDead;
    }
}
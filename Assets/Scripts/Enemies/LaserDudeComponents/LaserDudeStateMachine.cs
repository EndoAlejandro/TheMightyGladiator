using System;
using Enemies.EnemiesSharedStates;
using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.LaserDudeComponents
{
    public class LaserDudeStateMachine : FiniteStateBehaviour
    {
        private LaserDude _laserDude;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private NavigationSteering _navigationSteering;
        private LaserController _laserController;

        private EnemySpawn _spawn;
        private EnemyDeath _death;
        private EnemyChaseWalking _chaseWalking;

        protected override void References()
        {
            _laserDude = GetComponent<LaserDude>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _laserController = GetComponentInChildren<LaserController>();
            _navigationSteering = GetComponent<NavigationSteering>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_laserDude, _rigidbody);
            _chaseWalking = new EnemyChaseWalking(_laserDude, _rigidbody, _navigationSteering);
            var telegraph = new EnemyLaserTelegraph(_laserDude, _laserController);
            var attack = new EnemyLaserAttack(_laserDude, _laserController);
            var recover = new EnemyRecover(_laserDude);
            _death = new EnemyDeath(_laserDude, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);
            stateMachine.AddTransition(patrol, _chaseWalking, () => _laserDude.PlayerDetected);

            stateMachine.AddTransition(_chaseWalking, telegraph,
                () => _chaseWalking.CanSeePlayer && _chaseWalking.PlayerOnRange && _chaseWalking.PlayerInFront);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _chaseWalking, () => recover.Ended);

            stateMachine.AddTransition(_death, _chaseWalking, () => _death.Ended);
        }

        private void LaserDudeOnDead(Enemy enemy) => stateMachine.SetState(_death);

        private void OnEnable()
        {
            _laserDude.OnDead += LaserDudeOnDead;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable() => _laserDude.OnDead -= LaserDudeOnDead;
    }
}
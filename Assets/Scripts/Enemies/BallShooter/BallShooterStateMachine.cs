using Enemies.EnemiesSharedStates;
using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BallShooter
{
    public class BallShooterStateMachine : FiniteStateBehaviour
    {
        private BallShooter _ballShooter;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private EnemySpawn _spawn;
        private EnemyDeath _death;
        private EnemyChaseWalking _chaseWalking;

        protected override void References()
        {
            _ballShooter = GetComponent<BallShooter>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_ballShooter, _rigidbody);
            _chaseWalking = new EnemyChaseWalking(_ballShooter, _rigidbody);
            var telegraph = new EnemyTelegraph(_ballShooter);
            var attack = new BallShooterAttack(_ballShooter);
            var recover = new EnemyRecover(_ballShooter);
            _death = new EnemyDeath(_ballShooter, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);

            stateMachine.AddTransition(_chaseWalking, telegraph,
                () => _chaseWalking.CanSeePlayer && _chaseWalking.PlayerOnRange && _chaseWalking.CanSeePlayer);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _chaseWalking, () => recover.Ended);

            stateMachine.AddTransition(_death, _chaseWalking, () => _death.Ended);
        }

        private void BallShooterOnDead(Enemy enemy) => stateMachine.SetState(_death);
        private void BallShooterOnPlayerOnRange() => stateMachine.SetState(_chaseWalking);

        private void OnEnable()
        {
            _ballShooter.OnDead += BallShooterOnDead;
            _ballShooter.OnPlayerOnRange += BallShooterOnPlayerOnRange;
            stateMachine.SetState(_spawn);
        }


        private void OnDisable()
        {
            _ballShooter.OnPlayerOnRange -= BallShooterOnPlayerOnRange;
            _ballShooter.OnDead -= BallShooterOnDead;
        }
    }
}
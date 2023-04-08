using NavigationSteeringComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BallShooter
{
    [RequireComponent(typeof(NavigationSteering))]
    public class BallShooterStateMachine : FiniteStateBehaviour
    {
        private BallShooter _ballShooter;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private NavigationSteering _navigationSteering;

        private EnemySpawn _spawn;
        private EnemyDeath _death;
        private EnemyIdle _idle;

        protected override void References()
        {
            _ballShooter = GetComponent<BallShooter>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _navigationSteering = GetComponent<NavigationSteering>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_ballShooter, _rigidbody);
            _idle = new EnemyIdle(_ballShooter, _rigidbody, _navigationSteering);
            var telegraph = new EnemyTelegraph(_ballShooter);
            var attack = new BallShooterAttack(_ballShooter);
            var recover = new EnemyRecover(_ballShooter);
            _death = new EnemyDeath(_ballShooter, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);

            stateMachine.AddTransition(_idle, telegraph,
                () => _idle.CanSeePlayer && _idle.PlayerOnRange && _idle.CanSeePlayer);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, _idle, () => recover.Ended);

            stateMachine.AddTransition(_death, _idle, () => _death.Ended);
        }

        private void BallShooterOnDead(Enemy enemy) => stateMachine.SetState(_death);
        private void BallShooterOnPlayerOnRange() => stateMachine.SetState(_idle);

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
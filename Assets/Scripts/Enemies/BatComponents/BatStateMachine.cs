using System;
using CustomUtils;
using NavigationSteeringComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class BatStateMachine : FiniteStateBehaviour
    {
        private Bat _bat;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private NavigationSteering _navigationSteering;

        private EnemyIdle _idle;
        private EnemyStun _stun;
        private EnemyGetHit _getHit;
        private EnemyRecover _recover;
        private BatDeath _death;
        private EnemySpawn _spawn;

        private float _distance;

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var patrol = new EnemyPatrol(_bat, _rigidbody);
            // _idle = new BatIdle(_bat, _rigidbody, _navigationSteering);
            _idle = new EnemyIdle(_bat, _rigidbody, _navigationSteering);
            var telegraph = new BatTelegraph(_bat);
            var attack = new BatAttack(_bat, _rigidbody);
            _recover = new EnemyRecover(_bat);
            _stun = new EnemyStun(_bat);
            _getHit = new EnemyGetHit(_bat);
            _death = new BatDeath(_bat, _collider, _rigidbody);

            stateMachine.AddTransition(_spawn, patrol, () => _spawn.Ended);

            stateMachine.AddTransition(_idle, telegraph, () => _idle.PlayerOnRange && _idle.CanSeePlayer);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, _recover, () => attack.Ended);
            stateMachine.AddTransition(_recover, _idle, () => _recover.Ended);

            stateMachine.AddTransition(_getHit, _idle, () => _getHit.Ended);
            stateMachine.AddTransition(_stun, _idle, () => _stun.Ended);

            stateMachine.AddTransition(_death, _idle, () => _death.Ended);
        }

        private void OnEnable()
        {
            _bat.OnHit += BatOnHit;
            _bat.OnParry += BatOnParry;
            _bat.OnAttackCollision += BatOnAttackCollision;
            _bat.OnDead += BatOnDead;
            _bat.OnPlayerOnRange += BatOnPlayerOnRange;

            stateMachine.SetState(_spawn);
        }

        private void OnDisable()
        {
            _bat.OnHit -= BatOnHit;
            _bat.OnParry -= BatOnParry;
            _bat.OnAttackCollision -= BatOnAttackCollision;
            _bat.OnDead -= BatOnDead;
            _bat.OnPlayerOnRange -= BatOnPlayerOnRange;
        }

        private void BatOnPlayerOnRange() => stateMachine.SetState(_idle);
        private void BatOnDead(Enemy enemy) => stateMachine.SetState(_death);

        private void BatOnAttackCollision()
        {
            _rigidbody.AddForce(-transform.forward * 3f, ForceMode.VelocityChange);
            stateMachine.SetState(_recover);
        }

        private void BatOnParry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, Player.Instance.transform.position);
            _rigidbody.AddForce(direction * player.KnockBackForce, ForceMode.VelocityChange);
            stateMachine.SetState(_stun);
        }

        private void BatOnHit(Vector3 hitPoint, float knockBack)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, hitPoint);
            _rigidbody.AddForce(direction * knockBack, ForceMode.VelocityChange);
        }

        protected override void References()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bat = GetComponent<Bat>();
            _navigationSteering = GetComponent<NavigationSteering>();
            _collider = GetComponent<Collider>();
        }
    }
}
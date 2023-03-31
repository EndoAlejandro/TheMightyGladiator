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
        private Player _player;
        private NavigationSteering _navigationSteering;

        private BatIdle _idle;
        private EnemyStun _stun;
        private EnemyGetHit _getHit;
        private EnemyRecover _recover;

        private float _distance;

        protected override void StateMachine()
        {
            var idle = new BatIdle(_bat, _rigidbody, _player, _navigationSteering);
            var telegraph = new BatTelegraph(_bat, _player);
            var attack = new BatAttack(_bat, _rigidbody, _player);
            _recover = new EnemyRecover(_bat);
            _stun = new EnemyStun(_bat);
            _getHit = new EnemyGetHit(_bat);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, telegraph,
                () => idle.PlayerOnRange && idle.Ended && idle.CanSeePlayer);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, _recover, () => attack.Ended);
            stateMachine.AddTransition(_recover, idle, () => _recover.Ended);

            stateMachine.AddTransition(_getHit, idle, () => _getHit.Ended);
            stateMachine.AddTransition(_stun, idle, () => _stun.Ended);
        }

        private void OnEnable()
        {
            _bat.OnHit += BatOnHit;
            _bat.OnParry += BatOnParry;
            _bat.OnAttackCollision += BatOnAttackCollision;
        }

        private void OnDisable()
        {
            _bat.OnHit -= BatOnHit;
            _bat.OnParry -= BatOnParry;
            _bat.OnAttackCollision -= BatOnAttackCollision;
        }

        private void BatOnAttackCollision()
        {
            _rigidbody.AddForce(-transform.forward * 2f, ForceMode.VelocityChange);
            stateMachine.SetState(_recover);
        }

        private void BatOnParry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, _player.transform.position);
            _rigidbody.AddForce(direction * player.KnockBackForce, ForceMode.VelocityChange);
            stateMachine.SetState(_stun);
        }

        private void BatOnHit(Vector3 hitPoint, float knockBack)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, hitPoint);
            _rigidbody.AddForce(direction * knockBack, ForceMode.VelocityChange);

            if (stateMachine.CurrentState is not EnemyStun)
                stateMachine.SetState(_getHit);
        }

        protected override void References()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bat = GetComponent<Bat>();
            _player = FindObjectOfType<Player>();
            _navigationSteering = GetComponent<NavigationSteering>();
        }
    }
}
using System;
using CustomUtils;
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

        // States
        private BatIdle _idle;
        private EnemyStun _stun;

        private EnemyGetHit _getHit;
        // private BatDamage _damage;
        // private EnemyKnockBack _knockBack;

        private float _distance;

        protected override void StateMachine()
        {
            var idle = new BatIdle(_bat, _rigidbody, _player, _navigationSteering);
            var telegraph = new EnemyTelegraph(_bat);
            var attack = new BatAttack(_bat, _rigidbody, _player);
            var recover = new EnemyRecover(_bat);
            _stun = new EnemyStun(_bat);
            _getHit = new EnemyGetHit(_bat);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, telegraph,
                () => idle.PlayerOnRange && idle.Ended && idle.CanSeePlayer);
            stateMachine.AddTransition(telegraph, attack, () => telegraph.Ended);
            stateMachine.AddTransition(attack, recover, () => attack.Ended);
            stateMachine.AddTransition(recover, idle, () => recover.Ended);

            stateMachine.AddTransition(_getHit, idle, () => _getHit.Ended);
            stateMachine.AddTransition(_stun, idle, () => _stun.Ended);
        }

        private void OnEnable()
        {
            _bat.OnHit += BatOnHit;
            _bat.OnParry += BatOnParry;
        }

        private void OnDisable()
        {
            _bat.OnHit -= BatOnHit;
            _bat.OnParry -= BatOnParry;
        }

        private void BatOnParry(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, _player.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            stateMachine.SetState(_stun);
        }

        private void BatOnHit(Vector3 position)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
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
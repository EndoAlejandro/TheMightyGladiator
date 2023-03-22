using System;
using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.BatComponents
{
    public class BatStateMachine : FiniteStateBehaviour
    {
        private Bat _bat;
        private Rigidbody _rigidbody;
        private Player _player;
        private EnemyKnockBack _knockBack;
        private BatDamage _damage;
        private NavigationSteering _navigationSteering;

        private float _distance;

        protected override void Awake()
        {
            References();
            base.Awake();

            var idle = new BatIdle(_bat, _rigidbody, _player, _navigationSteering);
            var prepareAttack = new EnemyPrepareAttack(_bat);
            var attack = new BatAttack(_bat, _rigidbody, _player);
            var death = new BatDeath(_bat);
            _damage = new BatDamage(_bat, _rigidbody, _player);
            _knockBack = new EnemyKnockBack(_bat, _rigidbody, _player);

            stateMachine.SetState(idle);

            // Attack Sequence.
            stateMachine.AddTransition(idle, prepareAttack,
                () => idle.PlayerOnRange && idle.Ended && idle.CanSeePlayer);
            stateMachine.AddTransition(prepareAttack, attack, () => prepareAttack.Ended);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);

            // Get Hit
            stateMachine.AddTransition(_damage, _knockBack, () => true);
            stateMachine.AddTransition(_knockBack, idle, () => _knockBack.Ended);

            // Death
            // stateMachine.AddAnyTransition(death, () => !_bat.IsAlive);
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

        private void BatOnParry(Player player) => BatOnKnockBack(player);

        private void BatOnKnockBack(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, _player.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            stateMachine.SetState(_damage);
        }

        private void BatOnHit(Vector3 position)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            stateMachine.SetState(_damage);
        }

        private void References()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bat = GetComponent<Bat>();
            _player = FindObjectOfType<Player>();
            _navigationSteering = GetComponent<NavigationSteering>();
        }
    }
}
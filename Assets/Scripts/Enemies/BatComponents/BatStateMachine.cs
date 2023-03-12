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
        private EnemyKnockBack _knockBack;
        private EnemyDamage _damage;

        private float _distance;

        protected override void Awake()
        {
            References();
            base.Awake();

            var idle = new EnemyIdle(_bat, _rigidbody, _player);
            var prepareAttack = new EnemyPrepareAttack(_bat, _player);
            var attack = new EnemyAttack(_bat, _rigidbody, _player);
            _damage = new EnemyDamage(_bat, _rigidbody, _player);
            _knockBack = new EnemyKnockBack(_bat, _rigidbody, _player);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, prepareAttack, () => idle.PlayerOnRange && idle.Ended);
            stateMachine.AddTransition(prepareAttack, attack, () => prepareAttack.Ended);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);

            // stateMachine.AddTransition(_damage, _knockBack, () => true);
            stateMachine.AddTransition(_knockBack, idle, () => _knockBack.Ended);
        }

        private void Start()
        {
            _bat.OnHit += BatOnHit;
            _bat.OnKnockBack += BatOnKnockBack;
        }

        private void BatOnKnockBack(Player player) => stateMachine.SetState(_knockBack);
        private void BatOnHit(Player player) => stateMachine.SetState(_knockBack);

        private void References()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bat = GetComponent<Bat>();
            _player = FindObjectOfType<Player>();
        }
    }

    internal class EnemyDamage : IState
    {
        private readonly Bat _bat;
        private readonly Rigidbody _rigidbody;
        private readonly Player _player;

        private Vector3 _direction;
        private float _timer;

        public bool Ended => _timer <= 0f;

        public EnemyDamage(Bat bat, Rigidbody rigidbody, Player player)
        {
            _bat = bat;
            _rigidbody = rigidbody;
            _player = player;
        }

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}
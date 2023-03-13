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
        private EnemyDamage _damage;
        private NavigationSteering _navigationSteering;

        private float _distance;

        protected override void Awake()
        {
            References();
            base.Awake();

            var idle = new BatIdle(_bat, _rigidbody, _player, _navigationSteering);
            var prepareAttack = new EnemyPrepareAttack(_bat);
            var attack = new BatAttack(_bat, _rigidbody, _player);
            var death = new EnemyDeath(_bat);
            _damage = new EnemyDamage(_bat, _rigidbody, _player);
            _knockBack = new EnemyKnockBack(_bat, _rigidbody, _player);

            stateMachine.SetState(idle);

            // Attack Sequence.
            stateMachine.AddTransition(idle, prepareAttack, () => idle.PlayerOnRange && idle.Ended && idle.CanSeePlayer);
            stateMachine.AddTransition(prepareAttack, attack, () => prepareAttack.Ended);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);

            // Get Hit
            stateMachine.AddTransition(_damage, _knockBack, () => true);
            stateMachine.AddTransition(_knockBack, idle, () => _knockBack.Ended);

            // Death
            stateMachine.AddAnyTransition(death, () => !_bat.IsAlive);
        }

        private void Start()
        {
            _bat.OnHit += BatOnHit;
            _bat.OnKnockBack += BatOnKnockBack;
            _bat.OnParry += BatOnParry;
        }

        private void BatOnParry(Player player) => BatOnKnockBack(player);

        private void BatOnKnockBack(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, _player.transform.position);
            _rigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            stateMachine.SetState(_damage);
        }

        private void BatOnHit(Player player)
        {
            var direction = Utils.NormalizedFlatDirection(transform.position, _player.transform.position);
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

    public class EnemyDeath : IState
    {
        private readonly Bat _bat;
        private float _timer;
        public bool Ended => _timer <= 0f;
        public EnemyDeath(Bat bat) => _bat = bat;

        public void Tick()
        {
            _timer -= Time.deltaTime;
            if (Ended) GameObject.Destroy(_bat.gameObject);
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = _bat.DeathTime;
        public void OnExit() => GameObject.Destroy(_bat.gameObject);
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
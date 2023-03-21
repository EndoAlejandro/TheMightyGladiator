using System;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.WormComponents
{
    public class WormStateMachine : FiniteStateBehaviour
    {
        private Worm _worm;
        private Player _player;
        private WormStun _stun;

        protected override void Awake()
        {
            References();
            base.Awake();

            var idle = new WormIdle(_worm, _player);
            var attack = new WormAttack(_worm);
            var prepareAttack = new EnemyPrepareAttack(_worm);
            _stun = new WormStun(_worm);

            stateMachine.SetState(attack);

            stateMachine.AddTransition(idle, prepareAttack, () => idle.Ended);
            stateMachine.AddTransition(prepareAttack, attack, () => prepareAttack.Ended);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);

            stateMachine.AddTransition(_stun, idle, () => _stun.Ended);
        }

        private void OnEnable() => _worm.OnParry += WormOnParry;
        private void OnDisable() => _worm.OnParry -= WormOnParry;

        private void WormOnParry(Player player) => stateMachine.SetState(_stun);

        private void References()
        {
            _worm = GetComponent<Worm>();
            _player = FindObjectOfType<Player>();
        }
    }

    public class WormStun : IState
    {
        private readonly Worm _worm;
        private float _timer;
        public bool Ended => _timer <= 0;
        public WormStun(Worm worm) => _worm = worm;

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = 3f;

        public void OnExit()
        {
        }
    }
}
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    internal class BatDamage : IState
    {
        private readonly Bat _bat;
        private readonly Rigidbody _rigidbody;
        private readonly Player _player;

        private Vector3 _direction;
        private float _timer;

        public bool Ended => _timer <= 0f;

        public BatDamage(Bat bat, Rigidbody rigidbody, Player player)
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
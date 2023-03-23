using CustomUtils;
using Enemies.BatComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemyKnockBack : IState
    {
        private readonly Bat _bat;
        private readonly Rigidbody _rigidbody;
        private readonly Player _player;

        private Vector3 _direction;
        private float _timer;

        public bool Ended => _timer <= 0f;

        public EnemyKnockBack(Bat bat, Rigidbody rigidbody, Player player)
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
            _direction = Utils.NormalizedFlatDirection(_bat.transform.position, _player.transform.position);
            _rigidbody.velocity = Vector3.zero;
            _timer = _bat.StunTime;
            _rigidbody.AddForce(_direction * 2f, ForceMode.Impulse);
        }

        public void OnExit()
        {
        }
    }
}
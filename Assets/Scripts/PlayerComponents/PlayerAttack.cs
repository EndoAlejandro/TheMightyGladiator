using System;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAttack : IState
    {
        public static event Action<bool> OnAttackUpdated;

        private const float AttackAnimDuration = 0.25f;

        private readonly Player _player;
        private float _timer;

        public bool Ended => _timer <= 0f;

        public PlayerAttack(Player player) => _player = player;

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick() { }

        public void OnEnter()
        {
            OnAttackUpdated?.Invoke(true);
            _player.Attack();
            _timer = AttackAnimDuration;
        }

        public void OnExit()
        {
            OnAttackUpdated?.Invoke(false);
            _timer = 0f;
        }
    }
}
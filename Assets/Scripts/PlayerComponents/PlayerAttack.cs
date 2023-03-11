﻿using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAttack : IState
    {
        private const float AttackAnimDuration = 0.40f;
        private readonly Player _player;
        private float _timer;
        private bool _triggered;

        private float _hitBoxSize;
        private Vector3 _offset;
        public bool Ended => _timer <= 0f;

        public PlayerAttack(Player player)
        {
            _player = player;

            _hitBoxSize = 1f;
            _offset = Vector3.up * _hitBoxSize;
        }


        public void Tick()
        {
            _timer -= Time.deltaTime;

            if (_triggered) return;
            if (_timer <= AttackAnimDuration / 2)
            {
                _triggered = true;
                AttackDamage();
            }
        }

        private void AttackDamage()
        {
            var position = _player.transform.position;
            var forward = _player.transform.forward * _hitBoxSize;

            var results = Physics.OverlapBox(position + forward + _offset, Vector3.one * (_hitBoxSize / 2f),
                Quaternion.LookRotation(_player.transform.forward, _player.transform.up));
            foreach (var result in results)
            {
                Debug.Log(result.name);
            }
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _triggered = false;
            _timer = AttackAnimDuration;
        }

        public void OnExit()
        {
            _player.Attack();
            _timer = 0f;
        }
    }
}
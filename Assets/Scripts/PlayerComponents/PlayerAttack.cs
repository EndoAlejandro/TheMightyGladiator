﻿using CustomUtils;
using DungeonComponents;
using Enemies;
using Enemies.BatComponents;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAttack : IState
    {
        private const float AttackAnimDuration = 0.5f;

        private readonly Player _player;
        private float _timer;
        private bool _triggered;

        private readonly float _hitBoxSize;
        private readonly Vector3 _offset;

        private readonly Collider[] _results;

        public bool Ended => _timer <= 0f;

        public PlayerAttack(Player player)
        {
            _player = player;

            _hitBoxSize = _player.HitBoxSize;
            _offset = Vector3.up * _player.Height;

            _results = new Collider[20];
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
            var size = Physics.OverlapSphereNonAlloc(_player.transform.position + _offset,
                _hitBoxSize,
                _results,
                _player.AttackLayerMask);

            Vase closestVase = null;
            for (int i = 0; i < size; i++)
            {
                var result = _results[i];
                if (result.TryGetComponent(out Enemy enemy)) AttackEnemy(enemy, result);

                if (result.TryGetComponent(out Vase vase))
                {
                    if (!IsValidAngle(vase.transform) || !vase.CanBreak) continue;
                    if (closestVase == null) closestVase = vase;
                    else
                    {
                        var currentDistance = Vector3.Distance(vase.transform.position, _player.transform.position);
                        var closestDistance =
                            Vector3.Distance(closestVase.transform.position, _player.transform.position);
                        if (currentDistance < closestDistance) closestVase = vase;
                    }
                }
            }

            if (closestVase == null) return;
            closestVase.TakeDamage(1f);
            _player.DealDamage(closestVase.transform.position);
        }

        private void AttackEnemy(Enemy enemy, Collider result)
        {
            if (!enemy.IsAlive) return;
            if (!IsValidAngle(enemy.transform)) return;

            enemy.TakeDamage(_player.transform.position);
            _player.DealDamage(result.ClosestPoint(_player.transform.position));
        }

        public bool IsValidAngle(Transform target)
        {
            var direction = Utils.NormalizedFlatDirection(target.position, _player.transform.position);
            var angle = Vector3.Angle(direction, _player.transform.forward);
            return angle <= _player.AttackAngle;
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
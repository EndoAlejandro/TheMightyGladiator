using CustomUtils;
using Enemies.BatComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemyPrepareAttack : IState
    {
        private readonly Enemy _enemy;
        private float _timer;
        public bool Ended => _timer <= 0f;
        public EnemyPrepareAttack(Enemy enemy) => _enemy = enemy;
        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = _enemy.PrepareToAttackTime;

        public void OnExit()
        {
        }
    }
}
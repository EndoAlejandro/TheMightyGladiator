using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyAttack : IState
    {
        protected Enemy enemy;
        private float _parryTimer;

        public EnemyAttack(Enemy enemy) => this.enemy = enemy;

        public virtual void Tick()
        {
            _parryTimer -= Time.deltaTime;
            if (_parryTimer <= 0) enemy.SetCanBeParried(false);
        }

        public abstract void FixedTick();

        public virtual void OnEnter()
        {
            _parryTimer = enemy.ParryTimeWindow;
            enemy.SetCanBeParried(true);
            enemy.SetIsAttacking(true);
        }

        public virtual void OnExit()
        {
            enemy.SetCanBeParried(false);
            enemy.SetIsAttacking(false);
        }
    }
}
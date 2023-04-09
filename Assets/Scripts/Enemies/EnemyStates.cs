﻿using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemyStun : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyStun(Enemy enemy) => _enemy = enemy;

        public void OnEnter()
        {
            timer = _enemy.StunTime;
            _enemy.SetIsStun(true);
        }

        public override string ToString() => "Stun";

        public void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            _enemy.SetIsStun(false);
        }
    }

    public class EnemyGetHit : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyGetHit(Enemy enemy) => _enemy = enemy;
        public virtual void OnEnter() => timer = _enemy.GetHitTime;
        public override string ToString() => "GetHit";

        public virtual void FixedTick()
        {
        }
    }

    public abstract class EnemyAttack : IState
    {
        protected readonly Enemy enemy;
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

        public override string ToString() => "Attack";
    }
}
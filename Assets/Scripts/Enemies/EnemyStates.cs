using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemyStun : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyStun(Enemy enemy) => _enemy = enemy;
        public virtual void OnEnter() => timer = _enemy.StunTime;
        public override string ToString() => "Stun";

        public virtual void FixedTick()
        {
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

    public class EnemyRecover : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyRecover(Enemy enemy) => _enemy = enemy;
        public virtual void OnEnter() => timer = _enemy.RecoverTime;
        public override string ToString() => "Recover";

        public virtual void FixedTick()
        {
        }
    }

    public abstract class EnemyIdle : IState
    {
        public abstract void Tick();
        public abstract void FixedTick();
        public abstract void OnEnter();
        public abstract void OnExit();
        public override string ToString() => "Idle";
    }

    public class EnemyTelegraph : StateTimer, IState
    {
        private readonly Enemy _enemy;
        public EnemyTelegraph(Enemy enemy) => _enemy = enemy;
        public virtual void OnEnter() => timer = _enemy.TelegraphTime;
        public override string ToString() => "Telegraph";

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
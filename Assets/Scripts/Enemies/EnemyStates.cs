using CustomUtils;
using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawn : StateTimer, IState
    {
        public override string ToString() => "Spawn";

        public void FixedTick()
        {
        }

        public void OnEnter() => timer = Constants.SPAWN_TIME;
    }

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

    public class EnemyIdle : IState
    {
        public virtual void Tick()
        {
        }

        public virtual void FixedTick()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public override string ToString() => "Idle";
    }

    public class EnemyTelegraph : StateTimer, IState
    {
        protected readonly Enemy enemy;
        public EnemyTelegraph(Enemy enemy) => this.enemy = enemy;
        public virtual void OnEnter() => timer = enemy.TelegraphTime;
        public override string ToString() => "Telegraph";

        public virtual void FixedTick()
        {
        }
    }

    public class EnemyDeath : StateTimer, IState
    {
        public override string ToString() => "Death";
        protected readonly Enemy enemy;
        public EnemyDeath(Enemy enemy) => this.enemy = enemy;
        public virtual void OnEnter() => timer = enemy.DeathTime;

        public virtual void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            enemy.DeSpawn();
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
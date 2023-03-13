using StateMachineComponents;

namespace Enemies
{
    public abstract class EnemyAttack : IState
    {
        protected Enemy enemy;
        public abstract void Tick();
        public abstract void FixedTick();
        public virtual void OnEnter() => enemy.SetIsAttacking(true);
        public virtual void OnExit() => enemy.SetIsAttacking(false);
    }
}
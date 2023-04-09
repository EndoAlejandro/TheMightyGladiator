using StateMachineComponents;

namespace Enemies.EnemiesSharedStates
{
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
}
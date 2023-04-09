using StateMachineComponents;

namespace Enemies.EnemiesSharedStates
{
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
}
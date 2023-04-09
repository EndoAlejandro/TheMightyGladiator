using CustomUtils;
using StateMachineComponents;

namespace Enemies.EnemiesSharedStates
{
    public class EnemySpawn : StateTimer, IState
    {
        public override string ToString() => "Spawn";

        public void FixedTick()
        {
        }

        public void OnEnter() => timer = Constants.SPAWN_TIME;
    }
}
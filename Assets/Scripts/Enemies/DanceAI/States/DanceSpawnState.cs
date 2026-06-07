using StateMachineComponents;

namespace Enemies.DanceAI.States
{
    public class DanceSpawnState : StateTimer, IState
    {
        private readonly DanceEnemyController _controller;
        public DanceSpawnState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "Spawn";

        public void OnEnter() => timer = _controller.SpawnTime;

        // No movement intent => the steering body brakes to a smooth stop.
        public void FixedTick()
        {
        }
    }
}

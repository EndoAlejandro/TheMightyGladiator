using StateMachineComponents;

namespace Enemies.DanceAI.States
{
    public class DanceTelegraphState : StateTimer, IState
    {
        private readonly DanceEnemyController _controller;
        public DanceTelegraphState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "Telegraph";

        public void OnEnter() => timer = _controller.TelegraphTime;

        public override void Tick()
        {
            base.Tick();
            _controller.FacePlayer();
        }

        // No movement intent => brakes to a stop while winding up.
        public void FixedTick()
        {
        }
    }
}

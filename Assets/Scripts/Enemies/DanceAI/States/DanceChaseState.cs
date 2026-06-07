using StateMachineComponents;

namespace Enemies.DanceAI.States
{
    /// <summary>
    /// Goes for the player using a seek steering behaviour. Separation and
    /// wall-avoidance are layered on by the <see cref="SteeringBody"/>.
    /// </summary>
    public class DanceChaseState : IState
    {
        private readonly DanceEnemyController _controller;
        public DanceChaseState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "Chase";

        public void OnEnter() => _controller.ReleaseToken(); // give up any attack slot when re-chasing

        public void Tick() => _controller.FaceMovement();

        public void FixedTick() => _controller.SteerSeek(_controller.PlayerPosition);

        public void OnExit()
        {
        }
    }
}

using StateMachineComponents;

namespace Enemies.DanceAI.States
{
    /// <summary>
    /// "Dance": orbit the player at this enemy's own ring radius while waiting for the
    /// director's order. Once granted a token, arrive into attack range (the machine
    /// then transitions to Telegraph).
    /// </summary>
    public class DanceHoverState : IState
    {
        private readonly DanceEnemyController _controller;
        public DanceHoverState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "Dance";

        public void OnEnter() => _controller.IsDancing = true;

        public void Tick() => _controller.FacePlayer();

        public void FixedTick()
        {
            if (_controller.AttackGranted)
                _controller.SteerArrive(_controller.PlayerPosition); // our turn: step in
            else
                _controller.SteerOrbit();
        }

        public void OnExit() => _controller.IsDancing = false;
    }
}

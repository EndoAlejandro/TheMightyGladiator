using StateMachineComponents;

namespace Enemies.DanceAI.States
{
    /// <summary>
    /// Empty for now: just an exit time. Movement is intentionally left free here so a
    /// real attack can drive the body (e.g. <c>_controller.Drive(lungeVelocity)</c> or
    /// <c>_controller.SteerArrive(point)</c>) from FixedTick later. On exit it starts the
    /// cooldown and releases the director token so the next enemy gets its turn.
    /// </summary>
    public class DanceAttackState : StateTimer, IState
    {
        private readonly DanceEnemyController _controller;
        public DanceAttackState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "Attack";

        public void OnEnter() => timer = _controller.AttackTime;

        public override void Tick()
        {
            base.Tick();
            _controller.FacePlayer();
        }

        public void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            _controller.BeginAttackCooldown();
            _controller.ReleaseToken();
        }
    }
}

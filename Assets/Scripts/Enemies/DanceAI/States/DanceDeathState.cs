using FxComponents;
using StateMachineComponents;

namespace Enemies.DanceAI.States
{
    public class DanceDeathState : StateTimer, IState
    {
        private readonly DanceEnemyController _controller;
        public DanceDeathState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "Death";

        public void OnEnter()
        {
            timer = _controller.DeathTime;
            _controller.ReleaseToken();
            if (_controller.Collider != null) _controller.Collider.enabled = false;
            SfxManager.Instance.PlayFx(Sfx.EnemyDeath, _controller.transform.position);
        }

        // No movement intent => brakes to a stop while the death plays.
        public void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            VfxManager.Instance.PlayFx(Vfx.EnemySpawn, _controller.transform.position);
            _controller.Enemy.DeSpawn();
        }
    }
}

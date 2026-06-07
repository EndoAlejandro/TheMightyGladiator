using StateMachineComponents;
using UnityEngine;

namespace Enemies.DanceAI.States
{
    /// <summary>
    /// Pushed back along the hit direction for a fixed time. Uses the steering body's
    /// exact Drive (so it slides along walls via the navmesh clamp, never through them)
    /// and releases the attack token, which is what lets it interrupt a telegraph.
    /// </summary>
    public class DanceKnockBackState : StateTimer, IState
    {
        private readonly DanceEnemyController _controller;
        private Vector3 _velocity;

        public DanceKnockBackState(DanceEnemyController controller) => _controller = controller;

        public override string ToString() => "KnockBack";

        public void OnEnter()
        {
            timer = _controller.KnockbackTime;
            _velocity = _controller.PendingKnockDirection * _controller.KnockbackForce;
            _controller.ReleaseToken();
            _controller.Enemy.SetIsStun(true);
        }

        public void FixedTick()
        {
            _controller.Drive(_velocity);

            // decay to a full stop by the end of the knockback window
            var decay = _controller.KnockbackForce * (Time.fixedDeltaTime / Mathf.Max(0.01f, _controller.KnockbackTime));
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, decay);
        }

        public override void OnExit()
        {
            base.OnExit();
            _controller.Enemy.SetIsStun(false);
        }
    }
}

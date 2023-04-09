using StateMachineComponents;
using UnityEngine;

namespace Enemies.EnemiesSharedStates
{
    public class EnemyDeath : StateTimer, IState
    {
        public override string ToString() => "Death";
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;
        private readonly Collider _collider;

        public EnemyDeath(Enemy enemy, Rigidbody rigidbody, Collider collider)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;
            _collider = collider;
        }

        public virtual void OnEnter() => timer = _enemy.DeathTime;

        public virtual void FixedTick()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
            _enemy.DeSpawn();
        }
    }
}
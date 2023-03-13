using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.WormComponents
{
    public class WormIdle : IState
    {
        private readonly Worm _worm;
        private readonly Player _player;

        private float _timer;
        private NavMeshHit _hit;
        public bool Ended => _timer <= 0f;

        public WormIdle(Worm worm, Player player)
        {
            _worm = worm;
            _player = player;
        }

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _worm.SetColliderState(false);
            _timer = _worm.IdleTime;
        }

        public void OnExit()
        {
            var offset = Random.insideUnitSphere.With(y: 0);
            if (!NavMesh.SamplePosition(_player.transform.position + offset, out _hit, 5f, NavMesh.AllAreas))
                return;
            _worm.transform.position = _hit.position.With(y: 0f);
        }
    }
}
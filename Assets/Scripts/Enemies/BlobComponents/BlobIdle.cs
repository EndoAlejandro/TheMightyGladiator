using System.Linq;
using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.BlobComponents
{
    public class BlobIdle : IState
    {
        private readonly Blob _blob;
        private readonly Rigidbody _rigidbody;
        private readonly Player _player;
        private readonly NavMeshPath _path;

        private Vector3 _direction;
        private float _timer;
        private RaycastHit _hit;

        public bool Ended => _timer <= 0f;
        public bool CanSeePlayer { get; private set; }

        public BlobIdle(Blob blob, Rigidbody rigidbody, Player player)
        {
            _blob = blob;
            _rigidbody = rigidbody;
            _player = player;
            _path = new NavMeshPath();
        }

        public void Tick()
        {
            var playerDirection = Utils.NormalizedFlatDirection(_player.transform.position, _blob.transform.position);

            CanSeePlayer = Physics.Raycast(_blob.transform.position + Vector3.up * 0.1f, playerDirection, out _hit,
                               _blob.DetectionRange, _blob.PlayerLayerMask) &&
                           _hit.transform.TryGetComponent(out Player player);

            _timer -= Time.deltaTime;
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = _blob.MoveRate;

        public void OnExit()
        {
        }
    }
}
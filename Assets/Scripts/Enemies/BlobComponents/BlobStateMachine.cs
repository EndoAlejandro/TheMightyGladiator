using System.Linq;
using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.BlobComponents
{
    public class BlobStateMachine : FiniteStateBehaviour
    {
        private Blob _blob;
        private Rigidbody _rigidbody;
        private Collider _collider;

        protected override void StateMachine()
        {
            var idle = new BlobIdle(_blob, _rigidbody);
            var move = new BlobMove(_blob, _rigidbody);
            var attack = new BlobAttack(_blob);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, move, () => idle.Ended && !idle.CanSeePlayer);
            stateMachine.AddTransition(move, idle, () => move.Ended);

            stateMachine.AddTransition(idle, attack, () => idle.Ended && idle.CanSeePlayer);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);
        }

        protected override void References()
        {
            _blob = GetComponent<Blob>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }
    }

    public class BlobMove : IState
    {
        private readonly Blob _blob;
        private readonly Rigidbody _rigidbody;
        private readonly Player _player;
        private readonly NavMeshPath _path;

        private Vector3 _direction;

        public bool Ended { get; private set; }

        public BlobMove(Blob blob, Rigidbody rigidbody)
        {
            _blob = blob;
            _rigidbody = rigidbody;
            _path = new NavMeshPath();
        }

        public void Tick()
        {
            if (!Ended)
                Move();
            Ended = true;
        }

        private void Move()
        {
            _path.ClearCorners();
            NavMesh.CalculatePath(_blob.transform.position, _player.transform.position, NavMesh.AllAreas, _path);
            var length = _path.corners.Sum(corner => (corner - _blob.transform.position).magnitude);
            var noise = Random.insideUnitSphere.With(y: 0).normalized;

            if (length > _blob.StoppingDistance && _path.corners.Length > 0)
                _direction = (noise + Utils.NormalizedFlatDirection(_path.corners[1], _blob.transform.position))
                    .normalized;
            else
                _direction = noise;

            _rigidbody.AddForce(_direction * _blob.Speed, ForceMode.VelocityChange);
        }

        /*private void CheckForVisiblePlayer()
        {
            var playerDirection = Utils.NormalizedFlatDirection(_player.transform.position, _blob.transform.position);
            CanSeePlayer = Physics.Raycast(_blob.transform.position + Vector3.up * 0.25f, playerDirection,
                _blob.StoppingDistance, _blob.PlayerLayerMask);
        }*/

        public void FixedTick()
        {
        }

        public void OnEnter() => Ended = false;

        public void OnExit()
        {
        }
    }
}
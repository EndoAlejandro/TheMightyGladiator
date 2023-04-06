using Inputs;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerIdle : IState
    {
        private readonly Player _player;
        private readonly Rigidbody _rigidbody;
        private Vector3 _moveDirection;

        private Collider[] _results;

        public PlayerIdle(Player player, Rigidbody rigidbody)
        {
            _player = player;
            _rigidbody = rigidbody;

            _results = new Collider[50];
        }

        public void Tick()
        {
            if (!InputReader.Instance.Interact) return;

            var size = Physics.OverlapSphereNonAlloc(_player.transform.position, _player.HitBoxSize, _results);

            for (int i = 0; i < size; i++)
            {
                if (!_results[i].TryGetComponent(out Interactable interactable)) continue;
                interactable.Interact(_player);
                break;
            }
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}
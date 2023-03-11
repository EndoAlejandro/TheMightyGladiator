using System;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Right = Animator.StringToHash("Right");

        private PlayerStateMachine _playerStateMachine;

        private Animator _animator;
        private Rigidbody _rigidbody;

        private IState _state;
        private static readonly int Attack = Animator.StringToHash("Attack");

        private void Awake()
        {
            _playerStateMachine = GetComponentInParent<PlayerStateMachine>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponentInParent<Rigidbody>();
        }

        private void Start() => _playerStateMachine.OnEntityStateChanged += PlayerStateMachineOnEntityStateChanged;

        private void PlayerStateMachineOnEntityStateChanged(IState state)
        {
            _state = state;

            switch (_state)
            {
                case PlayerAttack playerAttack:
                    _animator.SetTrigger(Attack);
                    break;
            }
        }

        private void Update()
        {
            Walking();
            switch (_state)
            {
                case PlayerIdle playerIdle:
                    break;
            }
        }

        private void Walking()
        {
            var forward = Vector3.Dot(transform.forward, _rigidbody.velocity);
            var right = Vector3.Dot(transform.right, _rigidbody.velocity);

            _animator.SetFloat(Forward, forward);
            _animator.SetFloat(Right, right);
        }
    }
}
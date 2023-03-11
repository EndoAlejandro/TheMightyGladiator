using System;
using CustomUtils;
using StateMachineComponents;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Right = Animator.StringToHash("Right");

        [SerializeField] private GameObject slash;

        private PlayerStateMachine _playerStateMachine;

        private Animator _animator;
        private Rigidbody _rigidbody;

        private IState _state;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Shield = Animator.StringToHash("Shield");

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

            _animator.SetBool(Attack, _state is PlayerAttack);
            _animator.SetBool(Shield, _state is PlayerShield);
            switch (_state)
            {
                case PlayerAttack playerAttack:
                    // Instantiate(slash, transform.position.With(y: 1), Quaternion.LookRotation(transform.forward));
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
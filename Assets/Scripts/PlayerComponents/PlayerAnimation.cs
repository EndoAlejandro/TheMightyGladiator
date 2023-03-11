using System;
using CustomUtils;
using StateMachineComponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerComponents
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Right = Animator.StringToHash("Right");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int Shield = Animator.StringToHash("Shield");
        private static readonly int Dodge = Animator.StringToHash("Dodge");

        [SerializeField] private GameObject slash;

        private PlayerStateMachine _playerStateMachine;

        private Animator _animator;
        private Rigidbody _rigidbody;

        private IState _state;

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

            if (state is PlayerAttack)
            {
                _animator.SetTrigger(Attack);
                _animator.SetInteger(AttackIndex, (_animator.GetInteger(AttackIndex) + 1) % 2);
            }

            _animator.SetBool(Shield, _state is PlayerShield);
            _animator.SetBool(Dodge, _state is PlayerDodge);
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

        private void OnAnimatorIK(int layerIndex)
        {
            if (_state is not PlayerShield) return;

            var goal = AvatarIKGoal.LeftHand;
            _animator.SetIKPositionWeight(goal, 1f);
            _animator.SetIKRotationWeight(goal, 1f);

            _animator.SetIKPosition(goal, transform.position + transform.forward + Vector3.up);
            // _animator.SetIKRotation(goal, Quaternion.identity);
        }
    }
}
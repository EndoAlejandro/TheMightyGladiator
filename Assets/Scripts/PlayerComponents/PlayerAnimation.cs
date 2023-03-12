using StateMachineComponents;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Header("Shield")]
        [Range(0.01f, 1f)]
        [SerializeField] private float shieldDistanceIk = 0.5f;

        [SerializeField] private Transform shieldTransform;
        [SerializeField] private float shieldSizeAnimationSpeed = 10f;

        [SerializeField] private float shieldIdleScale = 1f;
        [SerializeField] private float shieldActiveScale = 1.5f;

        private Vector3 _shieldTargetScale;

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

        private void Start()
        {
            _shieldTargetScale = Vector3.one * shieldIdleScale;
            _playerStateMachine.OnEntityStateChanged += PlayerStateMachineOnEntityStateChanged;
        }

        private void PlayerStateMachineOnEntityStateChanged(IState state)
        {
            _state = state;

            switch (_state)
            {
                case PlayerAttack playerAttack:
                    _animator.SetTrigger(Attack);
                    _animator.SetInteger(AttackIndex, (_animator.GetInteger(AttackIndex) + 1) % 2);
                    break;
            }

            _shieldTargetScale = Vector3.one * (_state is PlayerShield ? shieldActiveScale : shieldIdleScale);
            _animator.SetBool(Shield, _state is PlayerShield);
            _animator.SetBool(Dodge, _state is PlayerDodge);
        }

        private void Update()
        {
            Walking();
            ManageShieldScale();
        }

        private void ManageShieldScale()
        {
            if (Vector3.Distance(shieldTransform.localScale, _shieldTargetScale) > 0.1f)
                shieldTransform.localScale = Vector3.Lerp(shieldTransform.localScale, _shieldTargetScale,
                    Time.deltaTime * shieldSizeAnimationSpeed);
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

            _animator.SetIKPosition(goal, transform.position + (transform.forward * shieldDistanceIk) + Vector3.up);
            // _animator.SetIKRotation(goal, Quaternion.identity);
        }
    }
}
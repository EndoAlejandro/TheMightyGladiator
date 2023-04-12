using System.Collections;
using Pooling;
using StateMachineComponents;
using UnityEngine;

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
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int ShieldHit = Animator.StringToHash("ShieldHit");
        private static readonly int Parry = Animator.StringToHash("Parry");
        private static readonly int Death = Animator.StringToHash("Death");

        [Header("Fx")]
        [SerializeField] private PoolAfterSeconds hitFx;

        [SerializeField] private GameObject slash;
        [SerializeField] private ParticleSystem slashParticle;

        [Header("Shield")]
        [Range(0.01f, 1f)]
        [SerializeField] private float shieldDistanceIk = 0.5f;

        [SerializeField] private Transform shieldTransform;
        [SerializeField] private float shieldSizeAnimationSpeed = 10f;

        [SerializeField] private float shieldIdleScale = 1f;
        [SerializeField] private float shieldActiveScale = 1.5f;
        [SerializeField] private float shieldPlacementOffset = 0.1f;

        private Vector3 _shieldTargetScale;

        private Player _player;
        private PlayerStateMachine _playerStateMachine;

        private Animator _animator;
        private Rigidbody _rigidbody;

        private IState _state;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();
            _playerStateMachine = GetComponentInParent<PlayerStateMachine>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponentInParent<Rigidbody>();
        }

        private void Start()
        {
            _shieldTargetScale = Vector3.one * shieldIdleScale;

            _player.OnHit += PlayerOnHit;
            _player.OnDealDamage += PlayerOnDealDamage;
            _player.OnParry += PlayerOnParry;
            _player.OnShieldHit += PlayerOnShieldHit;
            _playerStateMachine.OnEntityStateChanged += PlayerStateMachineOnEntityStateChanged;
        }

        private void PlayerOnDealDamage(Vector3 hitPoint) => hitFx.Get<PoolAfterSeconds>(hitPoint, Quaternion.identity);

        private void PlayerOnShieldHit()
        {
            _animator.SetTrigger(ShieldHit);
            if (_state is PlayerShield)
                shieldTransform.localScale = Vector3.one * 2;
        }

        private void OnParryEvent()
        {
        }

        private IEnumerator SlowTime()
        {
            MainCamera.Instance.Shake(2f);
            Time.timeScale = 0.05f;
            yield return new WaitForSecondsRealtime(1.5f);
            while (Time.timeScale < 0.95f)
            {
                Time.timeScale += Time.unscaledDeltaTime * 0.25f;
                yield return null;
            }

            Time.timeScale = 1f;
            GameManager.Instance.GameOver();
        }

        private void PlayerOnParry() => _animator.SetTrigger(Parry);
        private void PlayerOnHit() => _animator.SetTrigger(Hit);

        private void PlayerStateMachineOnEntityStateChanged(IState state)
        {
            _state = state;

            switch (_state)
            {
                case PlayerIdle playerIdle:
                    _animator.SetLayerWeight(1, 1f);
                    break;

                case PlayerAttack playerAttack:
                    _animator.SetLayerWeight(1, 1f);
                    slashParticle.Play();
                    _animator.SetTrigger(Attack);
                    _animator.SetInteger(AttackIndex, (_animator.GetInteger(AttackIndex) + 1) % 2);
                    break;

                case PlayerShield playerShield:
                    _animator.SetLayerWeight(1, 1f);
                    break;

                case PlayerDeath playerDeath:
                    _animator.SetLayerWeight(1, 0f);
                    _animator.SetTrigger(Death);
                    StartCoroutine(SlowTime());
                    break;

                case PlayerDodge:
                    _animator.SetLayerWeight(1, 0f);
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

            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit,
                    shieldDistanceIk))
                _animator.SetIKPosition(goal, hit.point + transform.forward * -shieldPlacementOffset);
            else
                _animator.SetIKPosition(goal, transform.position + (transform.forward * shieldDistanceIk) + Vector3.up);
        }
    }
}
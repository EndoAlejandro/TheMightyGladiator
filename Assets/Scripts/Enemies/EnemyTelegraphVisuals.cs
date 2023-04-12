using System;
using System.Collections;
using CustomUtils;
using Enemies.EnemiesSharedStates;
using StateMachineComponents;
using TMPro;
using UnityEngine;

namespace Enemies
{
    public class EnemyTelegraphVisuals : MonoBehaviour
    {
        private static readonly int IsOn = Animator.StringToHash("IsOn");

        [SerializeField] private TMP_Text visuals;
        [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1f);
        [SerializeField] private Color initialColor = Color.yellow;
        [SerializeField] private Color finalColor = Color.red;

        private Enemy _enemy;
        private FiniteStateBehaviour _stateMachine;
        private Animator _animator;
        private bool _isOn;
        private float _telegraphTime;


        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _stateMachine = GetComponentInParent<FiniteStateBehaviour>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _stateMachine.OnEntityStateChanged += StateMachineOnEntityStateChanged;
            _telegraphTime = _enemy.TelegraphTime;
            visuals.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!visuals.gameObject.activeInHierarchy) return;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        private void StateMachineOnEntityStateChanged(IState state)
        {
            switch (state)
            {
                case EnemyTelegraph telegraph:
                    StartCoroutine(AttackAnimation());
                    break;
                default:
                    if (!_isOn) return;
                    _isOn = false;
                    StopCoroutine(AttackAnimation());
                    visuals.gameObject.SetActive(false);
                    break;
            }
        }

        private IEnumerator AttackAnimation()
        {
            _isOn = true;
            transform.localScale = Vector3.one * scaleRange.x;
            visuals.SetText("!");
            visuals.gameObject.SetActive(true);
            var timer = _telegraphTime;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                var normalizedTimer = 1 - timer / _telegraphTime;
                visuals.color = Color.Lerp(initialColor, finalColor, normalizedTimer);
                transform.localScale = Vector3.one * scaleRange.GetPointInRange(normalizedTimer);
                yield return null;
            }

            _isOn = false;
            visuals.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_stateMachine != null)
                _stateMachine.OnEntityStateChanged -= StateMachineOnEntityStateChanged;
        }
    }
}
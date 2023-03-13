using System;
using Pooling;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.WormComponents
{
    public class WormAnimation : MonoBehaviour
    {
        [SerializeField] private PoolAfterSeconds rocksVfx;

        private Animator _animator;
        private Worm _worm;
        private WormStateMachine _stateMachine;
        private static readonly int Show = Animator.StringToHash("Show");
        private static readonly int Stun = Animator.StringToHash("Stun");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _worm = GetComponentInParent<Worm>();
            _stateMachine = GetComponentInParent<WormStateMachine>();
        }

        private void Start() => _stateMachine.OnEntityStateChanged += StateMachineOnEntityStateChanged;

        private void StateMachineOnEntityStateChanged(IState state)
        {
            _animator.SetBool(Stun, state is WormStun);
            switch (state)
            {
                case WormIdle wormIdle:
                    _animator.SetBool(Show, false);
                    break;
                case WormAttack wormAttack:
                    _animator.SetBool(Show, true);
                    break;
                case EnemyPrepareAttack prepareAttack:
                    rocksVfx.Get<PoolAfterSeconds>(transform.position, Quaternion.identity);
                    break;
            }
        }
    }
}
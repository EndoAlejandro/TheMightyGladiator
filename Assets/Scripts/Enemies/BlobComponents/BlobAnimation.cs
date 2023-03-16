using System;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BlobComponents
{
    public class BlobAnimation : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Move = Animator.StringToHash("Move");
        
        private Blob _blob;
        private BlobStateMachine _stateMachine;
        private Animator _animator;

        private void Awake()
        {
            _blob = GetComponent<Blob>();
            _stateMachine = GetComponent<BlobStateMachine>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _stateMachine.OnEntityStateChanged += StateMachineOnOnEntityStateChanged;
        }

        private void StateMachineOnOnEntityStateChanged(IState state)
        {
            switch (state)
            {
                case BlobAttack attack:
                    _animator.SetTrigger(Attack);
                    break;
                case BlobMove move:
                    _animator.SetTrigger(Move);
                    break;
            }
        }
    }
}

using StateMachineComponents;
using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBobAnimation : MonoBehaviour
    {
        private BigBobStateMachine _stateMachine;
        private Animator _animator;
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Attack = Animator.StringToHash("Attack");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = GetComponent<BigBobStateMachine>();
        }

        private void Start()
        {
            _stateMachine.OnEntityStateChanged += StateMachineOnEntityStateChanged;
        }

        private void StateMachineOnEntityStateChanged(IState state)
        {
            switch (state)
            {
                case BigBobInitialJump:
                    _animator.SetBool(Jump, true);
                    break;
                case BigBobEndJump:
                    _animator.SetBool(Jump, false);
                    break;
                case BigBobAttack:
                    _animator.SetTrigger(Attack);
                    break;
            }
        }
    }
}

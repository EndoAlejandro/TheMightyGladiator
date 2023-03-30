using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimation : MonoBehaviour
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Telegraph = Animator.StringToHash("Telegraph");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GetHit = Animator.StringToHash("GetHit");
        private static readonly int Stun = Animator.StringToHash("Stun");
        private static readonly int Recover = Animator.StringToHash("Recover");

        private FiniteStateBehaviour _stateMachine;
        private Animator _animator;

        private void Awake()
        {
            _stateMachine = GetComponent<FiniteStateBehaviour>();
            _animator = GetComponent<Animator>();
        }

        private void Start() => _stateMachine.OnEntityStateChanged += StateMachineOnEntityStateChanged;
        private void StateMachineOnEntityStateChanged(IState state) => _animator.SetTrigger(state.ToString());
    }
}
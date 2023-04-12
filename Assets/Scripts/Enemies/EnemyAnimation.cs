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
        private static readonly int Spawn = Animator.StringToHash("Spawn");

        private FiniteStateBehaviour _stateMachine;
        private Animator _animator;
        private Enemy _enemy;

        private void Awake()
        {
            _stateMachine = GetComponent<FiniteStateBehaviour>();
            _animator = GetComponent<Animator>();
            _enemy = GetComponent<Enemy>();
        }

        private void Start()
        {
            _stateMachine.OnEntityStateChanged += StateMachineOnEntityStateChanged;
            _enemy.OnHit += EnemyOnHit;
        }

        private void EnemyOnHit(Vector3 position, float knockBack) => _animator.SetTrigger(GetHit);

        private void StateMachineOnEntityStateChanged(IState state) => _animator.SetTrigger(state.ToString());
    }
}
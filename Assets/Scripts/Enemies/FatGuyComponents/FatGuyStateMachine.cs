using System;
using StateMachineComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.FatGuyComponents
{
    public class FatGuyStateMachine : FiniteStateBehaviour
    {
        private FatGuy _fatGuy;
        private Collider _collider;
        private Rigidbody _rigidbody;

        private EnemySpawn _spawn;
        private FatGuyRecover _recover;

        private int _patternIndex;

        protected override void References()
        {
            _fatGuy = GetComponent<FatGuy>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected override void StateMachine()
        {
            var idle = new FatGuyIdle(_fatGuy, _rigidbody);
            _spawn = new EnemySpawn();
            _recover = new FatGuyRecover(_fatGuy, NextAttackPattern);

            var dashTelegraph = new FatGuyTelegraph(_fatGuy);
            var dashAttack = new FatGuyDashAttack(_fatGuy, _rigidbody);

            var aoeTelegraph = new FatGuyTelegraph(_fatGuy, AoEFx);
            var aoeAttack = new FatGuyAoEAttack(_fatGuy);

            stateMachine.AddTransition(_spawn, idle, () => _spawn.Ended);
            stateMachine.AddTransition(idle, dashTelegraph, () => idle.Ended && _patternIndex < 2);
            stateMachine.AddTransition(idle, aoeTelegraph, () => idle.Ended && _patternIndex == 2);

            stateMachine.AddTransition(dashTelegraph, dashAttack, () => dashTelegraph.Ended && _patternIndex < 2);
            stateMachine.AddTransition(dashAttack, _recover, () => dashAttack.Ended);

            stateMachine.AddTransition(aoeTelegraph, aoeAttack, () => aoeTelegraph.Ended && _patternIndex == 2);
            stateMachine.AddTransition(aoeAttack, _recover, () => true);

            stateMachine.AddTransition(_recover, idle, () => _recover.Ended);
        }

        private void AoEFx()
        {
            VfxManager.Instance.PlayAoEFx(_fatGuy.transform.position, _fatGuy.TelegraphTime * 1.25f,
                _fatGuy.StoppingDistance);
        }

        private void NextAttackPattern() => _patternIndex = (_patternIndex + 1) % 3;

        private void OnEnable()
        {
            _fatGuy.OnAttackCollision += FatGuyOnAttackCollision;

            stateMachine.SetState(_spawn);
        }

        private void FatGuyOnAttackCollision()
        {
            _rigidbody.AddForce(-transform.forward * 3f, ForceMode.VelocityChange);
            stateMachine.SetState(_recover);
        }

        private void OnDisable()
        {
            _fatGuy.OnAttackCollision -= FatGuyOnAttackCollision;
        }
    }
}
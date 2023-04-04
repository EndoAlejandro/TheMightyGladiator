using System;
using StateMachineComponents;
using UnityEngine;
using VfxComponents;
using Random = UnityEngine.Random;

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

            var shotTelegraph = new FatGuyTelegraph(_fatGuy, customTime: 0.1f);
            var shotAttack = new FatGuyShot(_fatGuy);

            stateMachine.AddTransition(_spawn, idle, () => _spawn.Ended);

            stateMachine.AddTransition(idle, dashTelegraph, () => idle.Ended && _patternIndex == 0);
            stateMachine.AddTransition(dashTelegraph, dashAttack, () => dashTelegraph.Ended);
            stateMachine.AddTransition(dashAttack, _recover, () => dashAttack.Ended);

            stateMachine.AddTransition(idle, aoeTelegraph, () => idle.Ended && _patternIndex == 1);
            stateMachine.AddTransition(aoeTelegraph, aoeAttack, () => aoeTelegraph.Ended);
            stateMachine.AddTransition(aoeAttack, _recover, () => true);

            stateMachine.AddTransition(idle, shotTelegraph, () => idle.Ended && _patternIndex == 2);
            stateMachine.AddTransition(shotTelegraph, shotAttack, () => shotTelegraph.Ended);
            stateMachine.AddTransition(shotAttack, _recover, () => true);

            stateMachine.AddTransition(_recover, idle, () => _recover.Ended);
        }

        private void AoEFx()
        {
            VfxManager.Instance.PlayAoEFx(_fatGuy.transform.position, _fatGuy.TelegraphTime * 1.25f,
                _fatGuy.StoppingDistance);
        }

        private void NextAttackPattern()
        {
            int limit = _fatGuy.NormalizedHealth switch
            {
                < 0.3f => 3,
                < 0.6f => 2,
                _ => 1
            };
            _patternIndex = Random.Range(0, limit);
            Debug.Log($"Limit:{limit}:Index:{_patternIndex}");
        }

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
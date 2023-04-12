using Enemies.EnemiesSharedStates;
using FxComponents;
using StateMachineComponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.FatGuyComponents
{
    public class BossEnemyStateMachine : FiniteStateBehaviour
    {
        private BossEnemy _bossEnemy;
        private Collider _collider;
        private Rigidbody _rigidbody;

        private EnemySpawn _spawn;
        private BossRecover _recover;
        private BossDeath _death;

        private int _patternIndex;

        protected override void References()
        {
            _bossEnemy = GetComponent<BossEnemy>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        protected override void StateMachine()
        {
            var idle = new BossIdle(_bossEnemy, _rigidbody);
            _spawn = new EnemySpawn();
            _recover = new BossRecover(_bossEnemy, NextAttackPattern);
            _death = new BossDeath(_bossEnemy, _rigidbody, _collider);

            var dashTelegraph = new BossTelegraph(_bossEnemy);
            var dashAttack = new BossDashAttack(_bossEnemy, _rigidbody);

            var aoeTelegraph = new BossTelegraph(_bossEnemy, AoEFx);
            var aoeAttack = new BossAoeAttack(_bossEnemy);

            var shotTelegraph = new BossTelegraph(_bossEnemy, customTime: 0.1f);
            var shotAttack = new BossShot(_bossEnemy);

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
            stateMachine.AddTransition(_death, _spawn, () => _death.Ended);
        }

        private void AoEFx()
        {
            VfxManager.Instance.PlayAoEFx(_bossEnemy.transform.position, _bossEnemy.TelegraphTime * 1.25f,
                _bossEnemy.StoppingDistance);
        }

        private void NextAttackPattern()
        {
            int limit = _bossEnemy.NormalizedHealth switch
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
            _bossEnemy.OnAttackCollision += BossEnemyOnAttackCollision;
            _bossEnemy.OnDead += BossEnemyOnDead;

            stateMachine.SetState(_spawn);
        }

        private void BossEnemyOnDead(Enemy enemy) => stateMachine.SetState(_death);

        private void BossEnemyOnAttackCollision()
        {
            _rigidbody.AddForce(-transform.forward * 3f, ForceMode.VelocityChange);
            stateMachine.SetState(_recover);
        }

        private void OnDisable()
        {
            _bossEnemy.OnAttackCollision -= BossEnemyOnAttackCollision;
            _bossEnemy.OnDead -= BossEnemyOnDead;
        }
    }
}
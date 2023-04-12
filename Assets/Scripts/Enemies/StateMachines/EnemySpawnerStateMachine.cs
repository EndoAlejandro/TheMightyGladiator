using Enemies.EnemiesSharedStates;
using Enemies.SharedStates;
using Enemies.SpawnerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.StateMachines
{
    public class EnemySpawnerStateMachine : FiniteStateBehaviour
    {
        private SpawnerComponents.EnemySpawner _enemySpawner;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private EnemySpawn _spawn;
        private EnemyDeath _death;

        protected override void References()
        {
            _enemySpawner = GetComponent<SpawnerComponents.EnemySpawner>();
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var idle = new BlankState();
            var telegraph = new EnemyTelegraph(_enemySpawner);
            var spawnWave = new EnemySpawnerSpawn(_enemySpawner);
            _death = new EnemyDeath(_enemySpawner, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, idle, () => _spawn.Ended);
            stateMachine.AddTransition(idle, telegraph, () => _enemySpawner.SpawnedEnemies.Count == 0);
            stateMachine.AddTransition(telegraph, spawnWave, () => telegraph.Ended);
            stateMachine.AddTransition(spawnWave, idle, () => spawnWave.Ended);

            stateMachine.AddTransition(_death, idle, () => _death.Ended);
        }

        private void OnEnable()
        {
            _enemySpawner.OnDead += EnemySpawnerOnDead;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable() => _enemySpawner.OnDead -= EnemySpawnerOnDead;
        private void EnemySpawnerOnDead(Enemy enemy) => stateMachine.SetState(_death);
    }
}
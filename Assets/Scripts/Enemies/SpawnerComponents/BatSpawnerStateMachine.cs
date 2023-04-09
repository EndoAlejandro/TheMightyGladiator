using Enemies.EnemiesSharedStates;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.SpawnerComponents
{
    public class BatSpawnerStateMachine : FiniteStateBehaviour
    {
        private BatSpawner _batSpawner;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private EnemySpawn _spawn;
        private EnemyDeath _death;

        protected override void References()
        {
            _batSpawner = GetComponent<BatSpawner>();
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void StateMachine()
        {
            _spawn = new EnemySpawn();
            var idle = new BlankState();
            var telegraph = new EnemyTelegraph(_batSpawner);
            var spawnWave = new BatSpawnerSpawn(_batSpawner);
            _death = new EnemyDeath(_batSpawner, _rigidbody, _collider);

            stateMachine.AddTransition(_spawn, idle, () => _spawn.Ended);
            stateMachine.AddTransition(idle, telegraph, () => _batSpawner.SpawnedBats.Count == 0);
            stateMachine.AddTransition(telegraph, spawnWave, () => telegraph.Ended);
            stateMachine.AddTransition(spawnWave, idle, () => spawnWave.Ended);

            stateMachine.AddTransition(_death, idle, () => _death.Ended);
        }

        private void OnEnable()
        {
            _batSpawner.OnDead += BatSpawnerOnDead;
            stateMachine.SetState(_spawn);
        }

        private void OnDisable() => _batSpawner.OnDead -= BatSpawnerOnDead;
        private void BatSpawnerOnDead(Enemy enemy) => stateMachine.SetState(_death);
    }
}
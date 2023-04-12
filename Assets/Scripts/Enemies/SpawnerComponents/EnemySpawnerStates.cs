using System.Collections.Generic;
using System.Linq;
using CustomUtils;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.SpawnerComponents
{
    public class EnemySpawnerSpawn : StateTimer, IState
    {
        private readonly EnemySpawner _enemySpawner;
        private readonly List<Enemy> _spawnedEnemies;
        private Vector3[] _directions;

        public EnemySpawnerSpawn(EnemySpawner enemySpawner)
        {
            _enemySpawner = enemySpawner;
            _spawnedEnemies = new List<Enemy>();
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            timer = 2f;
            _directions = Utils.GetFanPatternDirections(_enemySpawner.transform, _enemySpawner.SpawnAmount, 360f);

            _enemySpawner.SpawnedEnemies.Clear();
            var enemies = _enemySpawner.Room.LevelData.Enemies.ToList();
            foreach (var enemy in enemies)
            {
                if (enemy is not EnemySpawner) continue;
                enemies.Remove(enemy);
                break;
            }

            var index = Random.Range(0, enemies.Count);
            foreach (var direction in _directions)
                _enemySpawner.SpawnEnemy(enemies[index], _enemySpawner.transform.position + direction * 1.5f);
        }

        public override void OnExit()
        {
            base.OnExit();
            _spawnedEnemies.Clear();
        }
    }
}
using System.Collections.Generic;
using DungeonComponents;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigRoom
{
    public class BigRoomController : HadesRoom
    {
        [SerializeField] private int maxEnemies = 5;
        [SerializeField] private float spawnRate = 1f;

        [SerializeField] private Enemy[] enemies;

        private List<Enemy> _spawnedEnemies;
        private SpawnPoint[] _spawnPoints;

        private float _timer;

        private void Awake()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>();
            _spawnedEnemies = new List<Enemy>();
        }

        private void Start() => SpawnEnemy();

        private void Update()
        {
            if (_timer > 0f) _timer -= Time.deltaTime;
            // if (_timer <= 0f && _spawnedEnemies.Count < maxEnemies)
        }

        private void SpawnEnemy()
        {
            _timer = spawnRate;
            for (int i = 0; i < maxEnemies; i++)
            {
                int spawnIndex = Random.Range(0, _spawnPoints.Length);
                int enemyIndex = Random.Range(0, enemies.Length);
                var enemy = enemies[enemyIndex]
                    .Get<Enemy>(_spawnPoints[spawnIndex].transform.position, Quaternion.identity);
                _spawnedEnemies.Add(enemy);

                enemy.OnDead += EnemyOnDead;
            }
        }

        private void EnemyOnDead(Enemy enemy)
        {
            _spawnedEnemies.Remove(enemy);
            enemy.OnDead -= EnemyOnDead;

            if (_spawnedEnemies.Count == 0)
                SpawnEnemy();
        }
    }
}
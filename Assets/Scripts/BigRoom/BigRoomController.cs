using System.Collections.Generic;
using DungeonComponents;
using Enemies;
using UnityEngine;

namespace BigRoom
{
    public class BigRoomController : HadesRoom
    {
        [SerializeField] private int maxEnemies = 2;
        [SerializeField] private int waves = 2;
        [SerializeField] private float spawnRate = 1f;
        [SerializeField] private Portal portal;

        private List<Enemy> _spawnedEnemies;
        private SpawnPoint[] _spawnPoints;

        private float _timer;

        private void Awake()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>();
            _spawnedEnemies = new List<Enemy>();

            portal.gameObject.SetActive(false);
        }

        private void Start() => SpawnEnemy();

        private void SpawnEnemy()
        {
            waves--;
            for (int i = 0; i < maxEnemies; i++)
            {
                int spawnIndex = Random.Range(0, _spawnPoints.Length);
                int enemyIndex = Random.Range(0, LevelData.Enemies.Length);
                var enemy = LevelData.Enemies[enemyIndex]
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
            {
                if (waves > 0) SpawnEnemy();
                else portal.gameObject.SetActive(true);
            }
        }
    }
}
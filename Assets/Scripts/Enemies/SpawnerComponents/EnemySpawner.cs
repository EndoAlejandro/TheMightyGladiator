using System.Collections;
using System.Collections.Generic;
using Enemies.BatComponents;
using FxComponents;
using UnityEngine;

namespace Enemies.SpawnerComponents
{
    public class EnemySpawner : Enemy
    {
        [Header("Spawn")]
        [SerializeField] private int spawnAmount = 2;

        public List<Enemy> SpawnedEnemies { get; private set; }
        public int SpawnAmount => spawnAmount;

        protected override void OnEnable()
        {
            base.OnEnable();
            SpawnedEnemies = new List<Enemy>();
        }

        public void SpawnEnemy(Enemy enemyToSpawn, Vector3 position) =>
            StartCoroutine(SpawnEnemyAfterSeconds(enemyToSpawn, position));

        private IEnumerator SpawnEnemyAfterSeconds(Enemy enemyToSpawn, Vector3 spawnPosition)
        {
            VfxManager.Instance.PlayFx(Vfx.SpawnCircle, spawnPosition);
            yield return new WaitForSeconds(1f);

            var spawnedEnemy = enemyToSpawn.Get<Enemy>(spawnPosition, Quaternion.identity);
            Room.RegisterEnemy(spawnedEnemy);
            spawnedEnemy.OnDead += SpawnedEnemyOnDead;
            SpawnedEnemies.Add(spawnedEnemy);
            VfxManager.Instance.PlayFx(Vfx.EnemySpawn, spawnPosition + Vector3.up);
        }

        private void SpawnedEnemyOnDead(Enemy enemy)
        {
            enemy.OnDead -= SpawnedEnemyOnDead;
            SpawnedEnemies.Remove(enemy);
        }
    }
}
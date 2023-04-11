using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.BatComponents;
using FxComponents;
using PlayerComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.SpawnerComponents
{
    public class BatSpawner : Enemy
    {
        [Header("Spawn")]
        [SerializeField] private Bat batPrefab;

        [SerializeField] private int spawnAmount = 2;

        public Bat BatPrefab => batPrefab;

        public List<Bat> SpawnedBats { get; private set; }

        public int SpawnAmount => spawnAmount;

        protected override void OnEnable()
        {
            base.OnEnable();
            SpawnedBats = new List<Bat>();
        }

        public void SpawnBat(Vector3 position) => StartCoroutine(SpawnEnemyAfterSeconds(position));

        private IEnumerator SpawnEnemyAfterSeconds(Vector3 spawnPosition)
        {
            VfxManager.Instance.PlayFx(Vfx.SpawnCircle, spawnPosition);
            yield return new WaitForSeconds(1f);

            var spawnedBat = batPrefab.Get<Bat>(spawnPosition, Quaternion.identity);
            Room.RegisterEnemy(spawnedBat);
            spawnedBat.OnDead += SpawnedBatOnDead;
            SpawnedBats.Add(spawnedBat);
            VfxManager.Instance.PlayFx(Vfx.EnemySpawn, spawnPosition + Vector3.up);
        }

        private void SpawnedBatOnDead(Enemy enemy)
        {
            enemy.OnDead -= SpawnedBatOnDead;
            SpawnedBats.Remove(enemy as Bat);
        }
    }
}
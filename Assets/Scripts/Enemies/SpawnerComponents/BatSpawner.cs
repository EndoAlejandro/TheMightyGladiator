using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.BatComponents;
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

        public override event Action<Enemy> OnDead;
        public override event Action<Vector3, float> OnHit;

        public Bat BatPrefab => batPrefab;

        public List<Bat> SpawnedBats { get; private set; }

        public int SpawnAmount => spawnAmount;

        protected override void OnEnable()
        {
            SpawnedBats = new List<Bat>();
        }

        public override void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
        {
            Health -= damage;
            VfxManager.Instance.PlayFloatingText(transform.position + Vector3.up * 2f, damage.ToString(".#"), IsStun);
            OnHit?.Invoke(hitPoint, knockBack);
            if (!IsAlive) OnDead?.Invoke(this);
        }

        public override void Parry(Player player)
        {
        }

        public void SpawnBat(Vector3 position) => StartCoroutine(SpawnEnemyAfterSeconds(position));

        private IEnumerator SpawnEnemyAfterSeconds(Vector3 spawnPosition)
        {
            VfxManager.Instance.PlayFx(Vfx.SpawnCircle, spawnPosition);
            yield return new WaitForSeconds(1f);

            var spawnedBat = batPrefab.Get<Bat>(spawnPosition, Quaternion.identity);
            room.RegisterEnemy(spawnedBat);
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
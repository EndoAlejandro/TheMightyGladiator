using System.Collections;
using Enemies;
using PlayerComponents;
using UnityEngine;
using VfxComponents;

namespace Rooms
{
    public abstract class BaseRoom : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private float spawnTime = 1f;
        protected Player Player { get; private set; }
        protected LevelData LevelData { get; private set; }

        public abstract void RegisterEnemy(Enemy enemy);

        public virtual void Setup(LevelData levelData, Player player)
        {
            LevelData = levelData;
            Player = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
            MainCamera.Instance.SetTarget(Player.transform);
        }

        protected IEnumerator SpawnEnemyAfterSeconds(Enemy enemyPrefab, Vector3 spawnPosition, float spawnCircleSize = 1f)
        {
            VfxManager.Instance.PlayFx(Vfx.SpawnCircle, spawnPosition, spawnCircleSize);
            yield return new WaitForSeconds(spawnTime);
            var enemy = enemyPrefab.Get<Enemy>(spawnPosition, Quaternion.identity);
            RegisterEnemy(enemy);
            VfxManager.Instance.PlayFx(Vfx.EnemySpawn, spawnPosition + Vector3.up);
        }
    }
}
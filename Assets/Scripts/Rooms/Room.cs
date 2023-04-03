using System.Collections;
using System.Collections.Generic;
using CustomUtils;
using Enemies;
using UnityEngine;
using Upgrades;
using VfxComponents;

namespace Rooms
{
    public class Room : BaseRoom
    {
        [SerializeField] private int maxEnemies = 2;
        [SerializeField] private int waves = 2;
        [SerializeField] private float spawnRate = 1f;
        [SerializeField] private Portal portal;
        [SerializeField] private Transform[] upgradePositions;

        private List<Enemy> _spawnedEnemies;
        private SpawnPoint[] _spawnPoints;

        private float _timer;
        private List<Upgrade> _upgrades;

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
            var spawnPointIndex = 0;
            _spawnPoints.Shuffle();
            for (int i = 0; i < maxEnemies; i++)
            {
                int enemyIndex = Random.Range(0, LevelData.Enemies.Length);
                StartCoroutine(
                    SpawnEnemyAfterSeconds(LevelData.Enemies[enemyIndex], _spawnPoints[i].transform.position));
                spawnPointIndex = (spawnPointIndex + 1) % _spawnPoints.Length;
            }
        }

        private IEnumerator SpawnEnemyAfterSeconds(Enemy enemyPrefab, Vector3 spawnPosition)
        {
            VfxManager.Instance.PlayFx(Vfx.NormalSpawn, spawnPosition);
            yield return new WaitForSeconds(spawnRate);
            var enemy = enemyPrefab.Get<Enemy>(spawnPosition, Quaternion.identity);
            RegisterEnemy(enemy);
        }

        public void RegisterEnemy(Enemy enemy)
        {
            enemy.Setup(this);
            _spawnedEnemies.Add(enemy);
            enemy.OnDead += EnemyOnDead;
        }

        private void EnemyOnDead(Enemy enemy)
        {
            _spawnedEnemies.Remove(enemy);
            enemy.OnDead -= EnemyOnDead;

            if (_spawnedEnemies.Count == 0)
            {
                if (waves > 0) SpawnEnemy();
                else SpawnUpgrades();
            }
        }

        private void SpawnUpgrades()
        {
            _upgrades = new List<Upgrade>();
            var upgrades = GameManager.Instance.GetUpgrades(upgradePositions.Length);
            for (int i = 0; i < upgrades.Count; i++)
            {
                var upgrade = Instantiate(upgrades[i], upgradePositions[i].position, Quaternion.identity);
                upgrade.OnUpgradeSelected += UpgradeOnUpgradeSelected;
                _upgrades.Add(upgrade);
            }
        }

        private void UpgradeOnUpgradeSelected(UpgradeType upgradeType)
        {
            foreach (var upgrade in _upgrades)
            {
                upgrade.OnUpgradeSelected -= UpgradeOnUpgradeSelected;
                Destroy(upgrade.gameObject);
            }

            portal.gameObject.SetActive(true);
        }
    }
}
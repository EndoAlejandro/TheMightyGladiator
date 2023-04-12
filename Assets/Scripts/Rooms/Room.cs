using System.Collections.Generic;
using CustomUtils;
using Enemies;
using PlayerComponents;
using UnityEngine;
using Upgrades;

namespace Rooms
{
    public class Room : BaseRoom
    {
        [SerializeField] private Portal portal;
        [SerializeField] private Transform[] upgradePositions;

        private List<Enemy> _spawnedEnemies;
        private SpawnPoint[] _spawnPoints;

        private int _waves;
        private float _timer;
        private List<Upgrade> _upgrades;

        protected override void Awake()
        {
            base.Awake();
            _spawnPoints = GetComponentsInChildren<SpawnPoint>();
            _spawnedEnemies = new List<Enemy>();

            portal.gameObject.SetActive(false);
        }

        public override void Setup(LevelData levelData, Player player)
        {
            base.Setup(levelData, player);
            _waves = levelData.Waves;
            NavMeshSurface.BuildNavMesh();
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            _waves--;
            var spawnPointIndex = 0;
            _spawnPoints.Shuffle();
            for (int i = 0; i < LevelData.EnemiesPerWave; i++)
            {
                int enemyIndex = Random.Range(0, LevelData.Enemies.Length);
                StartCoroutine(
                    SpawnEnemyAfterSeconds(LevelData.Enemies[enemyIndex], _spawnPoints[i].transform.position));
                spawnPointIndex = (spawnPointIndex + 1) % _spawnPoints.Length;
            }
        }

        public override void RegisterEnemy(Enemy enemy)
        {
            enemy.Setup(this);
            _spawnedEnemies.Add(enemy);
            enemy.OnDead += EnemyOnDead;
        }

        private void EnemyOnDead(Enemy enemy)
        {
            _spawnedEnemies.Remove(enemy);
            enemy.OnDead -= EnemyOnDead;

            if (_spawnedEnemies.Count > 0) return;
            
            if (_waves > 0) SpawnEnemy();
            else portal.gameObject.SetActive(true);
            //SpawnUpgrades();
        }

        /*private void SpawnUpgrades()
        {
            _upgrades = new List<Upgrade>();
            var upgrades = GameManager.Instance.GetUpgrades(upgradePositions.Length);
            for (int i = 0; i < upgrades.Count; i++)
            {
                var upgrade = Instantiate(upgrades[i], upgradePositions[i].position, Quaternion.identity);
                upgrade.OnUpgradeSelected += UpgradeOnUpgradeSelected;
                _upgrades.Add(upgrade);
            }
        }*/

        /*private void UpgradeOnUpgradeSelected(UpgradeType upgradeType)
        {
            foreach (var upgrade in _upgrades)
            {
                upgrade.OnUpgradeSelected -= UpgradeOnUpgradeSelected;
                Destroy(upgrade.gameObject);
            }

            portal.gameObject.SetActive(true);
        }*/
    }
}
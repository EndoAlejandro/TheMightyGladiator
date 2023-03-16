using System.Collections.Generic;
using CustomUtils;
using Enemies;
using PlayerComponents;
using Pooling;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private int maxAmount = 5;
    [SerializeField] private Enemy[] enemies;

    private Player _player;

    private List<Enemy> _spawnedEnemies;
    private int _enemiesInScene;
    private float _timer;
    private void Awake() => _player = FindObjectOfType<Player>();

    private void Update()
    {
        if (_timer > 0f) _timer -= Time.deltaTime;

        if (_enemiesInScene < maxAmount && _timer <= 0f)
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        _timer = spawnRate;
        _enemiesInScene++;
        var index = Random.Range(0, enemies.Length);
        var random = Random.insideUnitSphere.With(y: 0).normalized;
        var spawn = enemies[index].Get<Enemy>(_player.transform.position + random * 10f, Quaternion.identity);
        spawn.OnReturnToPool += SpawnOnReturnToPool;
    }

    private void SpawnOnReturnToPool(PooledMonoBehaviour _) => _enemiesInScene--;
}
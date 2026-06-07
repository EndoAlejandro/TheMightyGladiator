using System.Collections;
using Enemies.Combat;
using PlayerComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.DanceAI
{
    /// <summary>
    /// Spawns dancing enemies on the walkable navmesh around the player, keeping a
    /// live cap. Uses the existing pooling (Get&lt;T&gt; from PooledMonoBehaviour).
    /// Drop this on any object in a scene that has a baked NavMesh and a Player.
    /// </summary>
    public class DanceEnemySpawner : MonoBehaviour
    {
        [Tooltip("Prefab with MeleeEnemy/RangedEnemy + DanceEnemyController + (kinematic) Rigidbody + Collider.")]
        [SerializeField] private CombatEnemy _enemyPrefab;
        [SerializeField] private int _maxAlive = 8;
        [SerializeField] private float _spawnInterval = 1.5f;
        [SerializeField] private float _minSpawnRadius = 5f;
        [SerializeField] private float _maxSpawnRadius = 10f;
        [Tooltip("How far from a random point we will search for the nearest walkable navmesh spot.")]
        [SerializeField] private float _navSampleRadius = 2f;
        [SerializeField] private bool _spawnOnStart = true;

        private int _alive;

        private void Start()
        {
            if (_spawnOnStart) StartSpawning();
        }

        private void StartSpawning() => StartCoroutine(SpawnLoop());

        private IEnumerator SpawnLoop()
        {
            var wait = new WaitForSeconds(_spawnInterval);
            while (true)
            {
                if (_alive < _maxAlive && Player.Instance != null && TryGetSpawnPoint(out var point))
                    Spawn(point);

                yield return wait;
            }
        }

        private void Spawn(Vector3 point)
        {
            var enemy = _enemyPrefab.Get<CombatEnemy>(point, Quaternion.identity);
            if (enemy.TryGetComponent(out DanceEnemyController controller)) controller.WarpTo(point);

            _alive++;
            enemy.OnDeSpawn += OnEnemyDeSpawn;
        }

        private void OnEnemyDeSpawn(CombatEnemy enemy)
        {
            enemy.OnDeSpawn -= OnEnemyDeSpawn;
            _alive = Mathf.Max(0, _alive - 1);
        }

        private bool TryGetSpawnPoint(out Vector3 result)
        {
            var center = Player.Instance.transform.position;
            for (int i = 0; i < 30; i++)
            {
                var angle = Random.value * Mathf.PI * 2f;
                var radius = Random.Range(_minSpawnRadius, _maxSpawnRadius);
                var candidate = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

                if (NavMesh.SamplePosition(candidate, out var hit, _navSampleRadius, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = center;
            return false;
        }
    }
}

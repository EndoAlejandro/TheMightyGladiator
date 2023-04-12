using System.Collections;
using Enemies;
using FxComponents;
using Inputs;
using PlayerComponents;
using Unity.AI.Navigation;
using UnityEngine;

namespace Rooms
{
    [RequireComponent(typeof(NavMeshSurface))]
    public abstract class BaseRoom : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private float spawnTime = 1f;
        [SerializeField] private AudioClip music;
        protected Player Player { get; private set; }
        public LevelData LevelData { get; private set; }
        public NavMeshSurface NavMeshSurface { get; private set; }
        public float RandomSeed { get; private set; }
        
        protected virtual void Awake()
        {
            NavMeshSurface = GetComponent<NavMeshSurface>();
            NavMeshSurface.BuildNavMesh();
            RandomSeed = Random.Range(0f, 100f);
        }

        public abstract void RegisterEnemy(Enemy enemy);

        private void Update()
        {
            if (InputReader.Instance.Pause) GameManager.Instance.PauseToggle();
        }

        public virtual void Setup(LevelData levelData, Player player)
        {
            LevelData = levelData;
            Player = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
            MainCamera.Instance.SetTarget(Player.transform);
            SfxManager.Instance.PlayMusic(music);
        }

        protected IEnumerator SpawnEnemyAfterSeconds(Enemy enemyPrefab, Vector3 spawnPosition,
            float spawnCircleSize = 1f)
        {
            VfxManager.Instance.PlayFx(Vfx.SpawnCircle, spawnPosition, spawnCircleSize);
            yield return new WaitForSeconds(spawnTime);
            var enemy = enemyPrefab.Get<Enemy>(spawnPosition, Quaternion.identity);
            RegisterEnemy(enemy);
            VfxManager.Instance.PlayFx(Vfx.EnemySpawn, spawnPosition + Vector3.up);
        }
    }
}
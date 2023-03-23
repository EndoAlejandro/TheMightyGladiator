using UnityEngine;

namespace DungeonComponents
{
    public class VasePattern : MonoBehaviour
    {
        [SerializeField] private Transform spawnPointsContainer;

        public SpawnPoint[] SpawnPoints { get; private set; }

        private void Awake() => SpawnPoints = spawnPointsContainer.GetComponentsInChildren<SpawnPoint>();
    }
}
using PlayerComponents;
using UnityEngine;

namespace BigRoom
{
    public class HadesRoom : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;

        protected Player Player { get; private set; }
        protected LevelData LevelData { get; private set; }

        public virtual void Setup(LevelData levelData, Player player)
        {
            LevelData = levelData;
            Player = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
            MainCamera.Instance.SetTarget(Player.transform);
        }
    }
}
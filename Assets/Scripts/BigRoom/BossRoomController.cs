using PlayerComponents;
using UnityEngine;

namespace BigRoom
{
    public class BossRoomController : HadesRoom
    {
        [SerializeField] private Transform bossSpawnPoint;

        public override void Setup(LevelData levelData, Player player)
        {
            base.Setup(levelData, player);
            Instantiate(LevelData.Boss, bossSpawnPoint.position, Quaternion.identity);
        }
    }
}
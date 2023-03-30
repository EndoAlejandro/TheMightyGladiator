using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class BossRoom : BaseRoom
    {
        [SerializeField] private Transform bossSpawnPoint;

        public override void Setup(LevelData levelData, Player player)
        {
            base.Setup(levelData, player);
            Instantiate(LevelData.Boss, bossSpawnPoint.position, Quaternion.identity);
        }
    }
}
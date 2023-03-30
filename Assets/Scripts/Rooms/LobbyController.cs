using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        public void Setup(Player player) => Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
    }
}

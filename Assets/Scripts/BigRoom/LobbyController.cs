using PlayerComponents;
using UnityEngine;

namespace BigRoom
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        public void Setup(Player player) => Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
    }
}

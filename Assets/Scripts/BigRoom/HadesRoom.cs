using PlayerComponents;
using UnityEngine;

namespace BigRoom
{
    public class HadesRoom : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private 

        protected Player _player;

        public void Setup(Player player)
        {
            _player = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
            MainCamera.Instance.SetTarget(_player.transform);
        }
    }
}
using Rooms;
using UnityEngine;

namespace RoomTilingBehaviour
{
    public class FloorTile : MonoBehaviour
    {
        [SerializeField] private RandomizeFloorTile floorTilePrefab;
        [SerializeField] private SpikeHazard spikePrefab;

        private BaseRoom _room;

        private void Awake()
        {
            _room = GetComponentInParent<BaseRoom>();

            if (_room is Room)
            {
                var noise = Mathf.PerlinNoise(transform.position.x * _room.RandomSeed,
                    transform.position.z * _room.RandomSeed);
                if (noise > 0.75f) Instantiate(spikePrefab, transform);
                else Instantiate(floorTilePrefab, transform);
            }
            else
            {
                Instantiate(floorTilePrefab, transform);
            }
        }
    }
}
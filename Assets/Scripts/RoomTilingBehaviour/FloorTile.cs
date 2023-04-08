using UnityEngine;

namespace RoomTilingBehaviour
{
    public class FloorTile : MonoBehaviour
    {
        [SerializeField] private RandomizeFloorTile floorTilePrefab;
        [SerializeField] private SpikeHazard spikePrefab;

        private void Awake()
        {
            var noise = Mathf.PerlinNoise(transform.position.x, transform.position.y);
            if (noise > 0.75f) Instantiate(spikePrefab, transform);
            else Instantiate(floorTilePrefab, transform);
        }
    }
}
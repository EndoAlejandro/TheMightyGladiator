using UnityEngine;

namespace RoomTilingBehaviour
{
    public class RandomizeFloorTile : MonoBehaviour
    {
        [Range(0f, 1f)] [SerializeField] private float prob = .75f;
        [SerializeField] private GameObject[] variations;

        private void Awake()
        {
            var noise = Mathf.PerlinNoise(transform.position.x, transform.position.z);
            var lerp = Mathf.FloorToInt(Mathf.Lerp(0f, variations.Length, noise));
            foreach (var variation in variations) variation.SetActive(false);
            variations[lerp].SetActive(true);
        }
    }
}
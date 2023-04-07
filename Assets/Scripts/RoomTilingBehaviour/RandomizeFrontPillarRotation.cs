using UnityEngine;
using Random = UnityEngine.Random;

namespace RoomTilingBehaviour
{
    public class RandomizeFrontPillarRotation : MonoBehaviour
    {
        [SerializeField] private Transform pillarTransform;

        private void Awake()
        {
            var random = Random.Range(0, 4);
            pillarTransform.localRotation = Quaternion.Euler(0f, 90f * random, 0f);
        }
    }
}
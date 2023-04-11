using UnityEngine;

namespace Pooling
{
    public sealed class PoolAfterSeconds : PooledMonoBehaviour
    {
        [SerializeField] private float delay;
        [SerializeField] private AudioClip clip;
        private void OnEnable()
        {
            if (clip != null) SfxManager.Instance.PlayClip(clip);
            ReturnToPool(delay);
        }
    }
}
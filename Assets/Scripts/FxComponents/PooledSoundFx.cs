using Pooling;
using UnityEngine;

namespace FxComponents
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledSoundFx : PooledMonoBehaviour
    {
        private AudioSource _source;
        private void Awake() => _source = GetComponent<AudioSource>();

        public void Setup(AudioClip clip)
        {
            _source.PlayOneShot(clip);
            ReturnToPool(clip.length);
        }
    }
}
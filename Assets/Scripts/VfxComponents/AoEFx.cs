using System;
using Pooling;
using UnityEngine;

namespace VfxComponents
{
    public class AoEFx : PooledMonoBehaviour
    {
        private ParticleSystem[] _particles;

        private void Awake() => _particles = GetComponentsInChildren<ParticleSystem>();
        public void Setup(float duration, float size)
        {
            transform.localScale = Vector3.one * size;
            foreach (var particle in _particles)
            {
                var particlesMain = particle.main;
                particlesMain.startLifetime = duration;
                particle.Play();
            }
            ReturnToPool(duration);
        }
    }
}
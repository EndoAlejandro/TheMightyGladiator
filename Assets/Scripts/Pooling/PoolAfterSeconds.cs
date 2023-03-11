using UnityEngine;

namespace Pooling
{
    public class PoolAfterSeconds : PooledMonoBehaviour
    {
        [SerializeField] private float delay;
        protected virtual void OnEnable() => ReturnToPool(delay);
    }
}
using Pooling;
using UnityEngine;

namespace VfxComponents
{
    public class LaserVfx : PooledMonoBehaviour
    {
        private LineRenderer[] _lineRenderers;
        private void Awake() => _lineRenderers = GetComponentsInChildren<LineRenderer>();
        public void SetPosition(int index, Vector3 position)
        {
            foreach (var lineRenderer in _lineRenderers)
                lineRenderer.SetPosition(index, position);
        }
        public void TurnOff() => ReturnToPool();
    }
}
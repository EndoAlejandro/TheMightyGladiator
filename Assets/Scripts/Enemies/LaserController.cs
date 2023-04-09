using UnityEngine;

namespace Enemies
{
    public class LaserController : MonoBehaviour
    {
        [SerializeField] private LineRenderer telegraphLineRenderer;
        [SerializeField] private LineRenderer attackLineRenderer;
        public LineRenderer TelegraphLineRenderer => telegraphLineRenderer;
        public LineRenderer AttackLineRenderer => attackLineRenderer;
        public void SetTelegraphDisplayState(bool isActive) => TelegraphLineRenderer.gameObject.SetActive(isActive);
        public void SetAttackDisplayState(bool isActive) => AttackLineRenderer.gameObject.SetActive(isActive);
    }
}
using System.Collections;
using CustomUtils;
using Enemies;
using PlayerComponents;
using UnityEngine;

namespace NavigationSteeringComponents
{
    public class NavigationSteering : MonoBehaviour
    {
        [Range(1f, 360f)]
        [SerializeField] private float visionAngle = 360f;

        [Range(1, 100)]
        [SerializeField] private int segmentsAmount = 20;

        [SerializeField] private float detectionRange = 2f;
        [SerializeField] private float yOffset = 0.25f;
        [SerializeField] private LayerMask obstacleLayerMask;

        private Vector3 _playerDirection;

        private Vector3[] _directions;
        private DirectionWeight[] _sample;

        public DirectionWeight BestDirection { get; private set; }

        private void OnEnable() => StartCoroutine(FindPath());
        private void OnDisable() => StopAllCoroutines();

        

        private IEnumerator FindPath()
        {
            _playerDirection = Player.Instance == null
                ? transform.forward
                : Utils.NormalizedFlatDirection(Player.Instance.transform.position, transform.position);
            _directions = Utils.GetFanPatternDirections(transform, segmentsAmount, visionAngle);
            _sample = new DirectionWeight[_directions.Length];
            var w = 0f;
            for (int i = 0; i < _directions.Length; i++)
            {
                var result = Physics.Raycast(transform.position.Plus(y: yOffset), _directions[i],
                    out RaycastHit hit,
                    detectionRange,
                    obstacleLayerMask);

                if (result)
                {
                    if (hit.transform.TryGetComponent(out Player player))
                        w = 10;
                    else if (hit.transform.TryGetComponent(out Enemy enemy))
                        w = -1f;
                }
                else
                {
                    w = Utils.NormalizedDotProduct(_directions[i], _playerDirection) +
                        Utils.NormalizedDotProduct(_directions[i], transform.forward) + 1;
                }

                _sample[i] = new DirectionWeight(_directions[i], w);
            }


            BestDirection = new DirectionWeight(_directions[0], 0f);
            foreach (var direction in _sample)
            {
                if (direction.weight > BestDirection.weight)
                    BestDirection = direction;
            }

            yield return new WaitForSeconds(0.25f);
            StartCoroutine(FindPath());
        }

        private void OnDrawGizmosSelected()
        {
            var originPos = transform.position;
            originPos.y += yOffset;
            if (_sample == null) return;
            foreach (var direction in _sample)
                Gizmos.DrawLine(originPos, originPos + direction.direction * direction.weight);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(originPos, originPos + BestDirection.direction * BestDirection.weight);
        }
    }
}
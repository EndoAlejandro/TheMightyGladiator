using System;
using System.Collections;
using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies
{
    public class NavigationSteering : MonoBehaviour
    {
        [Range(1f, 360f)]
        [SerializeField] private float visionAngle = 270f;

        [Range(1, 100)]
        [SerializeField] private int segmentsAmount = 5;

        [SerializeField] private float detectionRange;
        [SerializeField] private float yOffset;
        [SerializeField] private LayerMask obstacleLayerMask;

        private Transform _playerTransform;

        private Vector3 _playerDirection;

        private Vector3[] _directions;
        private DirectionWeight[] _sample;

        public DirectionWeight BestDirection { get; private set; }

        private void Awake() => _playerTransform = FindObjectOfType<Player>().transform;
        private void Start() => StartCoroutine(FindPath());

        private IEnumerator FindPath()
        {
            _playerDirection = Utils.NormalizedFlatDirection(_playerTransform.position, transform.position);
            _directions = GetDetectionPositions();
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
                        Utils.NormalizedDotProduct(_directions[i], transform.forward);
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

        private Vector3[] GetDetectionPositions()
        {
            if (segmentsAmount <= 0 || visionAngle <= 0) return Array.Empty<Vector3>();

            var positions = new Vector3[segmentsAmount];
            var proportion = visionAngle / (segmentsAmount - 1);
            int index = 0;
            for (float i = -visionAngle / 2; i <= visionAngle / 2; i += proportion)
            {
                var radI = Mathf.Deg2Rad * (i - transform.localRotation.eulerAngles.y + 90f);
                var linePos = new Vector3(Mathf.Cos(radI), 0f, Mathf.Sin(radI));
                positions[index] = linePos.normalized;
                index++;
            }

            return positions;
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
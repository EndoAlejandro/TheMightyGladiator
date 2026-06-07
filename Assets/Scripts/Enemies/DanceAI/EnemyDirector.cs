using System.Collections.Generic;
using UnityEngine;

namespace Enemies.DanceAI
{
    /// <summary>
    /// Coordinates the dancing enemies: keeps a registry of every active enemy and,
    /// on a fixed cadence, hands out "attack tokens" so they take turns attacking the
    /// player. Round-robin, with a cap on how many may attack at once.
    /// Auto-creates itself if none is placed in the scene.
    /// </summary>
    public class EnemyDirector : MonoBehaviour
    {
        [Tooltip("How many enemies may be attacking (telegraph/attack) at the same time.")]
        [SerializeField] private int maxConcurrentAttackers = 1;
        [Tooltip("Seconds between attack-assignment decisions.")]
        [SerializeField] private float decisionInterval = 0.5f;

        private static EnemyDirector _instance;
        public static bool HasInstance => _instance != null;

        public static EnemyDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<EnemyDirector>();
                    if (_instance == null)
                        _instance = new GameObject(nameof(EnemyDirector)).AddComponent<EnemyDirector>();
                }

                return _instance;
            }
        }

        private readonly List<DanceEnemyController> _dancers = new List<DanceEnemyController>();
        private int _activeAttackers;
        private float _timer;
        private int _cursor;

        /// <summary>Live registry of enemies, used by <see cref="SteeringBody"/> for separation.</summary>
        public IReadOnlyList<DanceEnemyController> Dancers => _dancers;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        public void Register(DanceEnemyController dancer)
        {
            if (!_dancers.Contains(dancer)) _dancers.Add(dancer);
        }

        public void Unregister(DanceEnemyController dancer) => _dancers.Remove(dancer);

        public void OnTokenReleased() => _activeAttackers = Mathf.Max(0, _activeAttackers - 1);

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            _timer = decisionInterval;

            if (_dancers.Count == 0)
            {
                _activeAttackers = 0;
                return;
            }

            var attempts = _dancers.Count;
            while (_activeAttackers < maxConcurrentAttackers && attempts-- > 0)
            {
                _cursor = (_cursor + 1) % _dancers.Count;
                var candidate = _dancers[_cursor];
                if (candidate == null || !candidate.IsEligibleToAttack) continue;

                candidate.GrantToken();
                _activeAttackers++;
            }
        }
    }
}

using CustomUtils;
using PlayerComponents;
using UnityEngine;

namespace Enemies.BlobComponents
{
    public class Blob : Enemy
    {
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private float moveRate = 1.5f;

        [Header("Attack pattern")]
        [Range(1, 20)]
        [SerializeField] private int shotsAmount = 4;

        [Range(1f, 360f)]
        [SerializeField] private float shotAngle = 180f;

        [SerializeField] private Bullet bullet;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float detectionRange = 10f;
        private float _health;
        public Bullet Bullet => bullet;

        public float MoveRate => moveRate;
        public LayerMask PlayerLayerMask => playerLayerMask;
        public override bool IsAlive => true;
        public float BulletSpeed => bulletSpeed;
        public float DetectionRange => detectionRange;

        private void OnEnable() => _health = maxHealth;

        public override void TakeDamage(Vector3 position)
        {
            _health--;

            if (_health <= 0) ReturnToPool();
        }

        public override void Parry(Player player)
        {
            // TODO: Implement parry behaviour.
        }

        public Vector3[] GetFanPatternDirections() => Utils.GetFanPatternDirections(transform, shotsAmount, shotAngle);
    }
}
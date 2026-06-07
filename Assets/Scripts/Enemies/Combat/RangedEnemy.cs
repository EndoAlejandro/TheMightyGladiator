using UnityEngine;

namespace Enemies.Combat
{
    /// <summary>A projectile attacker: it fires one or more bullets along its facing.</summary>
    public class RangedEnemy : CombatEnemy
    {
        [Header("Ranged Attack")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _bulletSpeed = 8f;
        [SerializeField] private int _bulletsPerShot = 1;
        [Tooltip("Total spread (degrees) the bullets fan across.")]
        [SerializeField] private float _spreadAngle = 0f;

        public override void PerformAttack()
        {
            if (_bulletPrefab == null) return;

            var origin = _muzzle != null ? _muzzle.position : transform.position + Vector3.up * 0.5f;

            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var t = _bulletsPerShot > 1 ? i / (float)(_bulletsPerShot - 1) : 0.5f;
                var yaw = Mathf.Lerp(-_spreadAngle * 0.5f, _spreadAngle * 0.5f, t);
                var direction = Quaternion.Euler(0f, yaw, 0f) * transform.forward;

                var bullet = _bulletPrefab.Get<Bullet>(origin, Quaternion.LookRotation(direction));
                bullet.Setup(direction, _bulletSpeed, Damage);
            }
        }
    }
}

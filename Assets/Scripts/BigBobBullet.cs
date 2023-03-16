using CustomUtils;
using Pooling;
using UnityEngine;

public class BigBobBullet : PooledMonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;

    private Rigidbody _rigidbody;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        var directions = Utils.GetFanPatternDirections(transform, 6, 360);

        foreach (var direction in directions)
        {
            var position = transform.position.With(y: 0.5f);
            var bullet = bulletPrefab.Get<Bullet>(position, Quaternion.identity);
            bullet.Setup(direction, 5f);
        }

        ReturnToPool();
    }

    public void Setup(Vector3 target, float angle)
    {
        var velocity = Utils.BallisticVelocity(transform.position, target, angle);
        _rigidbody.velocity = velocity;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _rigidbody.velocity = Vector3.zero;
    }
}
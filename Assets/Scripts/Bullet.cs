using Enemies;
using Enemies.BlobComponents;
using PlayerComponents;
using Pooling;
using UnityEngine;

public class Bullet : PooledMonoBehaviour
{
    private Rigidbody _rigidbody;

    private float _speed;
    private Vector3 _direction;

    private bool _reflected;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void OnEnable() => ReturnToPool(10f);

    public void Setup(Vector3 direction, float speed)
    {
        _direction = direction;
        _speed = speed;
    }

    private void FixedUpdate() => _rigidbody.velocity = _direction * _speed;

    public void Parry()
    {
        _reflected = true;
        _direction *= -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bullet bullet)) return;

        if (_reflected)
        {
            if (other.TryGetComponent(out Player player)) return;

            if (other.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage(transform.position);

            ReturnToPool();
        }
        else
        {
            if (other.TryGetComponent(out Enemy enemy)) return;

            if (other.TryGetComponent(out Player player) && player.GetDamageFromEnemy(transform.position))
                player.TakeDamage(transform.position, 1);
            
            ReturnToPool();
        }
    }

    protected override void OnDisable()
    {
        _reflected = false;
        base.OnDisable();
    }
}
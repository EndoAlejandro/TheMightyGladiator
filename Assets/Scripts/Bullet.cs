using CustomUtils;
using Enemies;
using FxComponents;
using PlayerComponents;
using Pooling;
using UnityEngine;

public class Bullet : PooledMonoBehaviour, IDealDamage
{
    public Vector3 Velocity => _rigidbody == null ? Vector3.zero : _rigidbody.velocity;

    private Rigidbody _rigidbody;

    private float _speed;
    private float _turnSpeed;

    private Vector3 _playerDirection;
    private Vector3 _direction;

    private bool _reflected;
    private bool _followPlayer;
    private float _timer;

    public int Damage { get; private set; }

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void OnEnable() => ReturnToPool(10f);

    public void Setup(Vector3 direction, float speed, int damage, bool followPlayer = false, float turnSpeed = 1f)
    {
        _timer = .25f;
        _direction = direction;
        _speed = speed;
        Damage = damage;
        _followPlayer = followPlayer;
        _turnSpeed = turnSpeed;
    }

    private void FixedUpdate()
    {
        if (_timer > 0f) _timer -= Time.deltaTime;
        if (_followPlayer && Player.Instance != null && !_reflected && _timer <= 0f)
        {
            _playerDirection = Utils.NormalizedFlatDirection(Player.Instance.transform.position, transform.position);
            _rigidbody.velocity =
                Vector3.Lerp(_rigidbody.velocity.normalized, _playerDirection, Time.deltaTime * _turnSpeed) * _speed;
        }
        else
            _rigidbody.velocity = _direction * _speed;

        transform.forward = _rigidbody.velocity.normalized;
    }

    public void Parry(int newDamage)
    {
        Damage = newDamage;
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
                enemy.TakeDamage(other.ClosestPoint(transform.position), Damage);

            OnHit();
        }
        else
        {
            if (other.TryGetComponent(out Enemy enemy)) return;

            if (other.TryGetComponent(out Player player))
                player.TryToGetDamageFromEnemy(this);

            OnHit();
        }
    }

    private void OnHit()
    {
        SfxManager.Instance.PlayFx(Sfx.BulletHit, transform.position);
        VfxManager.Instance.PlayFx(Vfx.BombHit, transform.position + Vector3.up * 0.5f);
        ReturnToPool();
    }

    protected override void OnDisable()
    {
        _reflected = false;
        base.OnDisable();
    }
}
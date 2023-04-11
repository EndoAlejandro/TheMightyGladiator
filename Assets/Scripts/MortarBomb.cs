using CustomUtils;
using Enemies;
using PlayerComponents;
using Pooling;
using UnityEngine;
using VfxComponents;

public class MortarBomb : PooledMonoBehaviour, IDealDamage
{
    [SerializeField] private int damage = 1;

    [Header("Branching")]
    [SerializeField] private bool branchAtExplosion;

    [SerializeField] private Bullet bulletPrefab;

    [SerializeField] private int bulletsAmount = 6;
    private Rigidbody _rigidbody;

    private PooledMonoBehaviour _hitPredictionFx;
    public int Damage => damage;

    private Collider[] _results;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _results = new Collider[10];
    }

    private void Update()
    {
        transform.forward = _rigidbody.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent(out Enemy enemy)) return;
        FxManager.Instance.PlayFx(Vfx.BombHit, transform.position + Vector3.up * 0.5f);
        if (_hitPredictionFx != null) _hitPredictionFx.ReturnToPool();

        var size = Physics.OverlapSphereNonAlloc(transform.position, 1f, _results);
        for (int i = 0; i < size; i++)
        {
            if (!_results[i].TryGetComponent(out Player player)) continue;
            player.TryToGetDamageFromEnemy(this);
        }

        if (branchAtExplosion) Branching();
        ReturnToPool();
    }

    private void Branching()
    {
        var directions = Utils.GetFanPatternDirections(transform, bulletsAmount, 360);

        foreach (var direction in directions)
        {
            var position = transform.position.With(y: 0.5f);
            var bullet = bulletPrefab.Get<Bullet>(position, Quaternion.identity);
            bullet.Setup(direction, 5f, Damage);
        }
    }

    public void Setup(Vector3 target, float angle)
    {
        var velocity = Utils.BallisticVelocity(transform.position, target, angle);
        _hitPredictionFx = FxManager.Instance.PlayHitPointPredictionFx(target);
        _rigidbody.velocity = velocity;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _rigidbody.velocity = Vector3.zero;
    }
}
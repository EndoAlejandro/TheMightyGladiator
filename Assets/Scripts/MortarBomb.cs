using System;
using CustomUtils;
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

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
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
        _hitPredictionFx = VfxManager.Instance.PlayHitPointPredictionFx(target);
        _rigidbody.velocity = velocity;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_hitPredictionFx != null) _hitPredictionFx.ReturnToPool();
        _rigidbody.velocity = Vector3.zero;
    }
}
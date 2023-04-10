using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBob : Enemy
    {
        private Collider _collider;

        private void Awake() => _collider = GetComponent<Collider>();

        public override void SetIsAttacking(bool isAttacking)
        {
            base.SetIsAttacking(isAttacking);
            _collider.isTrigger = isAttacking;
        }

        private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position + Vector3.up * YOffset, AoeRadius);
    }
}
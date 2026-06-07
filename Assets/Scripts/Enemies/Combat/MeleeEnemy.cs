using PlayerComponents;
using UnityEngine;

namespace Enemies.Combat
{
    /// <summary>A close-range attacker: it hits the player if they are inside its arc.</summary>
    public class MeleeEnemy : CombatEnemy
    {
        [Header("Melee Attack")]
        [field: SerializeField] public float AttackRange { get; private set; } = 2f;

        [Range(0f, 360f)] [SerializeField] private float _attackAngle = 120f;

        public override void PerformAttack()
        {
            var player = Player.Instance;
            if (player == null) return;

            var toPlayer = player.transform.position - transform.position;
            toPlayer.y = 0f;

            if (toPlayer.magnitude > AttackRange) return;
            if (Vector3.Angle(transform.forward, toPlayer) > _attackAngle * 0.5f) return;

            player.TryToGetDamageFromEnemy(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }
}
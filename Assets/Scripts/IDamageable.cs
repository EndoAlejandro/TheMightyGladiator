using UnityEngine;

/// <summary>
/// Anything the player can hit. Decouples the player's attacks from concrete enemy
/// types so old (<see cref="Enemies.Enemy"/>) and new (<see cref="Enemies.Combat.CombatEnemy"/>)
/// enemies can both take damage.
/// </summary>
public interface IDamageable
{
    Transform transform { get; }
    bool IsAlive { get; }
    bool IsStun { get; }
    void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0f);
}

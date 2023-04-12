using UnityEngine;

public interface IDealDamage
{
    Transform transform { get; }
    Vector3 Velocity { get; }
    int Damage { get; }
}
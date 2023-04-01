using PlayerComponents;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/New Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;

    [SerializeField] private int health = 10;
    [SerializeField] private float immunityTime = 0.25f;

    public int MaxHealth => maxHealth;
    public int Health => health;
    public float ImmunityTime => immunityTime;

    [Header("Locomotion")]
    [SerializeField] private float walkSpeed = 6f;

    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSpeed = 100f;

    public float WalkSpeed => walkSpeed;
    public float Acceleration => acceleration;
    public float RotationSpeed => rotationSpeed;

    [Header("Dodge")]
    [SerializeField] private float dodgeRate = 0.25f;

    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDistance = 3f;

    public float DodgeRate => dodgeRate;
    public float DodgeSpeed => dodgeSpeed;
    public float DodgeDistance => dodgeDistance;

    [Header("Attack")]
    [SerializeField] private float damage = 1f;

    [Range(0f, 1f)] [SerializeField] private float criticalProbability = 0.05f;
    [Range(1f, 2f)] [SerializeField] private float criticalDamage = 1.2f;
    [SerializeField] private float attackRate = 0.25f;

    public float Damage => damage;
    public float CriticalProbability => criticalProbability;
    public float CriticalDamage => criticalDamage;
    public float AttackRate => attackRate;
    
    [Header("Defend")]
    [SerializeField] private float defendRate = 1f;

    [SerializeField] private float parryTime = 0.25f;
    [SerializeField] private float knockBackForce = 10f;

    public float DefendRate => defendRate;
    public float ParryTime => parryTime;
    public float KnockBackForce => knockBackForce;
    
    public void SaveData(Player player)
    {
        maxHealth = player.MaxHealth;
        health = player.Health;
        immunityTime = player.ImmunityTime;
        walkSpeed = player.WalkSpeed;
        acceleration = player.Acceleration;
        rotationSpeed = player.RotationSpeed;
        dodgeRate = player.DodgeRate;
        dodgeSpeed = player.DodgeSpeed;
        dodgeDistance = player.DodgeDistance;
        damage = player.Damage;
        criticalProbability = player.CriticalProbability;
        criticalDamage = player.CriticalDamage;
        attackRate = player.AttackRate;
        defendRate = player.DefendRate;
        parryTime = player.ParryTime;
        knockBackForce = player.KnockBackForce;
    }
}
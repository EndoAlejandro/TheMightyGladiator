using UnityEngine;

public class WallDestruction : MonoBehaviour, IDamageable
{
    public bool IsAlive { get; private set; } = true;

    public bool IsStun => false;

    [SerializeField] private float _maxHealth = 3f;

    private float _currentHealth;

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(Vector3 hitPoint, float damage, float knockBack = 0)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
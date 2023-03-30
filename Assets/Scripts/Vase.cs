using UnityEngine;

public class Vase : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float _health;
    public bool CanBreak { get; private set; }

    private void OnEnable()
    {
        _health = maxHealth;
        CanBreak = true;
    }

    public void GetDamageFromPlayer(float damageAmount)
    {
        _health -= damageAmount;

        if (_health <= 0)
        {
            CanBreak = true;
            Destroy(gameObject);
        }
    }
}
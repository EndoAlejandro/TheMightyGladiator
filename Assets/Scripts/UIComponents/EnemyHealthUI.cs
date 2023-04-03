using System;
using Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class EnemyHealthUI : MonoBehaviour
    {
        [SerializeField] private GameObject background;
        [SerializeField] private Image healthBar;
        private Enemy _enemy;
        private void Awake() => _enemy = GetComponentInParent<Enemy>();
        private void OnEnable() => _enemy.OnHit += EnemyOnHit;
        private void Update() => transform.rotation = Quaternion.identity;

        private void OnDisable()
        {
            background.SetActive(false);
            _enemy.OnHit -= EnemyOnHit;
        }

        private void EnemyOnHit(Vector3 position, float knockBack)
        {
            background.SetActive(true);
            UpdateHealthBar();
        }

        private void UpdateHealthBar() => healthBar.fillAmount = _enemy.Health / _enemy.MaxHealth;
    }
}
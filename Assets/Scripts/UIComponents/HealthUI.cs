using PlayerComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private TMP_Text healthText;

        private void Start()
        {
            UpdateHealthDisplay();
            Player.Instance.OnHit += PlayerOnHit;
            Player.Instance.OnUpgrade += PlayerOnUpgrade;
        }

        private void PlayerOnUpgrade() => UpdateHealthDisplay();

        private void PlayerOnHit() => UpdateHealthDisplay();

        private void UpdateHealthDisplay()
        {
            var player = Player.Instance;
            var normalizedHealth = (float)player.Health / player.MaxHealth;
            healthBar.fillAmount = normalizedHealth;
            healthText.SetText(player.Health.ToString(".#") + " / " + player.MaxHealth.ToString(".#"));
        }
    }
}
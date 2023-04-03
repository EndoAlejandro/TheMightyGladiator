using System;
using PlayerComponents;
using UnityEngine;

namespace Upgrades
{
    public class Upgrade : Interactable
    {
        public event Action<UpgradeType> OnUpgradeSelected;
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private bool destroyAfterUse;
        [SerializeField] private float value;

        [TextArea]
        [SerializeField] private string description;

        public UpgradeType UpgradeType => upgradeType;

        public override void Interact(Player player)
        {
            switch (upgradeType)
            {
                case UpgradeType.Heal:
                    player.Heal(Mathf.RoundToInt(value));
                    break;
                case UpgradeType.MaxHeal:
                    player.IncreaseMaxHealth(Mathf.RoundToInt(value));
                    break;
                case UpgradeType.Damage:
                    player.IncreaseDamage(value);
                    break;
                case UpgradeType.CriticalDamage:
                    player.IncreaseCriticalDamage(value);
                    break;
                case UpgradeType.CriticalProb:
                    player.IncreaseCriticalProbability(value);
                    break;
            }

            OnUpgradeSelected?.Invoke(UpgradeType);
            
            if (destroyAfterUse) Destroy(gameObject);
        }
    }
}
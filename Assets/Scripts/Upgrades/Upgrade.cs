using System;
using FxComponents;
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

        private void Start()
        {
            SfxManager.Instance.PlayFx(Sfx.UpgradeSpawn, transform.position);
            VfxManager.Instance.PlayFx(Vfx.UpgradeSpawn, transform.position);
        }

        public override void Interact(Player player)
        {
            switch (upgradeType)
            {
                case UpgradeType.Heal:
                    VfxManager.Instance.PlayFx(Vfx.Heal, transform.position);
                    SfxManager.Instance.PlayFx(Sfx.Heal, transform.position);
                    player.Heal(Mathf.RoundToInt(value));
                    break;
                case UpgradeType.MaxHeal:
                    VfxManager.Instance.PlayFx(Vfx.Upgrade, transform.position);
                    SfxManager.Instance.PlayFx(Sfx.Upgrade, transform.position);
                    player.IncreaseMaxHealth(Mathf.RoundToInt(value));
                    break;
                case UpgradeType.Damage:
                    VfxManager.Instance.PlayFx(Vfx.Upgrade, transform.position);
                    SfxManager.Instance.PlayFx(Sfx.Upgrade, transform.position);
                    player.IncreaseDamage(value);
                    break;
                case UpgradeType.CriticalDamage:
                    SfxManager.Instance.PlayFx(Sfx.Upgrade, transform.position);
                    VfxManager.Instance.PlayFx(Vfx.Upgrade, transform.position);
                    player.IncreaseCriticalDamage(value);
                    break;
                case UpgradeType.CriticalProb:
                    SfxManager.Instance.PlayFx(Sfx.Upgrade, transform.position);
                    VfxManager.Instance.PlayFx(Vfx.Upgrade, transform.position);
                    player.IncreaseCriticalProbability(value);
                    break;
            }

            OnUpgradeSelected?.Invoke(UpgradeType);

            Destroy(gameObject);
        }
    }
}
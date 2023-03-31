using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class Portal : Interactable
    {
        [SerializeField] private PortalType portalType;
        public override void Interact(Player player) => GameManager.Instance.PortalActivated(portalType);
    }
}
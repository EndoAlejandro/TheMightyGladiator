using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class Portal : Interactable
    {
        private void OnEnable() => transform.rotation = Quaternion.identity;
        public override void Interact(Player player) => GameManager.Instance.PortalActivated();
    }
}
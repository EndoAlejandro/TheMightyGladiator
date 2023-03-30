using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class Portal : MonoBehaviour, IInteractable
    {
        [SerializeField] private PortalType portalType;
        private CustomToolTip _toolTip;
        private void Awake() => _toolTip = GetComponentInChildren<CustomToolTip>();

        private void OnTriggerEnter(Collider other)
        {
            if (_toolTip == null) return;
            if (other.TryGetComponent(out Player player))
                _toolTip.SetVisibility(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_toolTip == null) return;
            if (other.TryGetComponent(out Player player))
                _toolTip.SetVisibility(false);
        }

        public void Interact(Player player) => GameManager.Instance.PortalActivated(portalType);
    }
}
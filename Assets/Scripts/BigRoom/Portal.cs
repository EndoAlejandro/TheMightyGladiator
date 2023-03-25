using PlayerComponents;
using UnityEngine;

namespace BigRoom
{
    public class Portal : MonoBehaviour, IInteractable
    {
        private CustomToolTip _toolTip;

        private void Awake() => _toolTip = GetComponentInChildren<CustomToolTip>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
                _toolTip.SetVisibility(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
                _toolTip.SetVisibility(false);
        }

        public void Interact(Player player) => GameManager.Instance.NextLevel();
    }
}
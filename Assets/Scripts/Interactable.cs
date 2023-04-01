using PlayerComponents;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private CustomToolTip _toolTip;
    protected virtual void Awake() => _toolTip = GetComponentInChildren<CustomToolTip>();
    public abstract void Interact(Player player);

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
}
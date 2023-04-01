using PlayerComponents;

namespace Rooms
{
    public class Portal : Interactable
    {
        public override void Interact(Player player) => GameManager.Instance.PortalActivated();
    }
}
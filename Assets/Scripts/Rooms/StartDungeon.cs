using PlayerComponents;

namespace Rooms
{
    public class StartDungeon : Interactable
    {
        public override void Interact(Player player) => GameManager.Instance.StartDungeonPortalActivated();
    }
}
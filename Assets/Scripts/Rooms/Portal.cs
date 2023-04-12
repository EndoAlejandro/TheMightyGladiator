using System.Collections;
using FxComponents;
using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class Portal : Interactable
    {
        private void OnEnable() => transform.rotation = Quaternion.identity;
        public override void Interact(Player player) => StartCoroutine(TeleportSequence(player));

        private IEnumerator TeleportSequence(Player player)
        {
            player.Teleport(1f);
            SfxManager.Instance.PlayFx(Sfx.PortalActive, transform.position);
            yield return new WaitForSeconds(1f);
            GameManager.Instance.PortalActivated();
        }
    }
}
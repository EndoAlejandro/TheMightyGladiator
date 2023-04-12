using System;
using System.Collections;
using FxComponents;
using PlayerComponents;
using UnityEngine;

namespace RoomTilingBehaviour
{
    public class SpikeHazard : MonoBehaviour, IDealDamage
    {
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private float activeTime = 4f;
        [SerializeField] private int damage = 1;
        [SerializeField] private GameObject[] spikes;
        private Collider _collider;
        private bool _isActive;
        public Vector3 Velocity => Vector3.zero;
        public int Damage => damage;

        private void Awake() => _collider = GetComponent<Collider>();
        private void Start() => SetSpikeActivationState(false);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player player)) return;
            if (_isActive) player.TryToGetDamageFromEnemy(this, true);
            else StartCoroutine(ActivateSpikes());
        }

        private IEnumerator ActivateSpikes()
        {
            _collider.enabled = false;
            VfxManager.Instance.PlayFx(Vfx.SpikesPrepare, transform.position);
            SfxManager.Instance.PlayFx(Sfx.SpikesPrepare, transform.position);
            yield return new WaitForSeconds(activationTime);
            _isActive = true;
            _collider.enabled = true;
            SetSpikeActivationState(true);
            VfxManager.Instance.PlayFx(Vfx.SpikesUp, transform.position);
            SfxManager.Instance.PlayFx(Sfx.SpikesUp, transform.position);
            StartCoroutine(DeactivateSpikes());
        }

        private IEnumerator DeactivateSpikes()
        {
            yield return new WaitForSeconds(activeTime);
            _isActive = false;
            SetSpikeActivationState(false);
        }

        private void SetSpikeActivationState(bool isActive)
        {
            foreach (var spike in spikes) spike.SetActive(isActive);
        }
    }
}
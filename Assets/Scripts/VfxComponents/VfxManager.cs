using System.Collections.Generic;
using CustomUtils;
using Pooling;
using UnityEngine;

namespace VfxComponents
{
    public class VfxManager : Singleton<VfxManager>
    {
        [SerializeField] private FloatingText floatingText;
        [SerializeField] private PoolAfterSeconds swordHit;
        [SerializeField] private PoolAfterSeconds swordCriticalHit;
        [SerializeField] private PoolAfterSeconds playerGetHit;
        [SerializeField] private PoolAfterSeconds normalSpawn;

        private Dictionary<Vfx, PoolAfterSeconds> _listedVfx;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            _listedVfx = new Dictionary<Vfx, PoolAfterSeconds>
            {
                { Vfx.Sword, swordHit },
                { Vfx.SwordCritical, swordCriticalHit },
                { Vfx.PlayerHit, playerGetHit },
                { Vfx.NormalSpawn, normalSpawn },
            };
        }

        public void PlayFx(Vfx fx, Vector3 position) =>
            _listedVfx[fx].Get<PoolAfterSeconds>(position, Quaternion.identity);

        public void PlayFloatingText(Vector3 position, string damage, bool isCritical)
        {
            var text = floatingText.Get<FloatingText>(position, Quaternion.identity);
            text.Setup(damage, isCritical);
        }
    }
}
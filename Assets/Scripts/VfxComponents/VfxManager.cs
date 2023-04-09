using System;
using System.Collections.Generic;
using CustomUtils;
using Pooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace VfxComponents
{
    public class VfxManager : Singleton<VfxManager>
    {
        [SerializeField] private FloatingText floatingText;
        [SerializeField] private AoEFx aoeFx;
        [SerializeField] private PoolAfterSeconds swordHit;
        [SerializeField] private PoolAfterSeconds swordCriticalHit;
        [SerializeField] private PoolAfterSeconds playerGetHit;
        [SerializeField] private PoolAfterSeconds explosion;
        [SerializeField] private PoolAfterSeconds bombHitVfx;
        [SerializeField] private PoolAfterSeconds splashVfx;
        [SerializeField] private PoolAfterSeconds splashJumpVfx;
        [SerializeField] private PooledMonoBehaviour hitPointPrediction;

        [FormerlySerializedAs("normalSpawn")] [SerializeField]
        private PoolAfterSeconds normalSpawnCircle;

        [SerializeField] private PoolAfterSeconds enemySpawn;

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
                { Vfx.SpawnCircle, normalSpawnCircle },
                { Vfx.EnemySpawn, enemySpawn },
                { Vfx.Explosion, explosion },
                { Vfx.BombHit , bombHitVfx},
                { Vfx.Splash , splashVfx},
                { Vfx.SplashJump, splashJumpVfx}
            };
        }

        public void PlayFx(Vfx fx, Vector3 position, float scale = 1f)
        {
            var vfx = _listedVfx[fx].Get<PoolAfterSeconds>(position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * scale;
        }

        public void PlayAoEFx(Vector3 position, float duration = 1f, float size = 1f)
        {
            var aoe = aoeFx.Get<AoEFx>(position, Quaternion.identity);
            aoe.Setup(duration, size);
        }

        public PooledMonoBehaviour PlayHitPointPredictionFx(Vector3 position)
        {
            var pooled = hitPointPrediction.Get<PooledMonoBehaviour>(position, Quaternion.identity);
            return pooled;
        }

        public void PlayFloatingText(Vector3 position, string damage, bool isCritical)
        {
            var text = floatingText.Get<FloatingText>(position, Quaternion.identity);
            text.Setup(damage, isCritical);
        }
    }
}
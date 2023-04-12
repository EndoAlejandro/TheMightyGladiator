using System;
using Enemies;
using FxComponents;
using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class BossRoom : BaseRoom
    {
        [SerializeField] private Transform bossSpawnPoint;
        [SerializeField] private Portal portal;

        private Enemy _boss;

        protected override void Awake()
        {
            base.Awake();
            portal.gameObject.SetActive(false);
        }

        private void Start() =>
            StartCoroutine(SpawnEnemyAfterSeconds(LevelData.Boss, bossSpawnPoint.position, 3f));

        public override void RegisterEnemy(Enemy enemy)
        {
            _boss = enemy;
            _boss.Setup(this);
            _boss.OnDead += BossOnDead;
        }

        private void BossOnDead(Enemy boss)
        {
            _boss.OnDead -= BossOnDead;
            SfxManager.Instance.PlayFx(Sfx.RoomCleared, Player.Instance.transform.position);
            portal.gameObject.SetActive(true);
        }
    }
}
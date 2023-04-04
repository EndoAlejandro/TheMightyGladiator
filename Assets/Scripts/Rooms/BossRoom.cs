using System;
using Enemies;
using PlayerComponents;
using UnityEngine;

namespace Rooms
{
    public class BossRoom : BaseRoom
    {
        [SerializeField] private Transform bossSpawnPoint;
        [SerializeField] private Portal portal;

        private Enemy _boss;

        private void Awake()
        {
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
            portal.gameObject.SetActive(true);
        }
    }
}
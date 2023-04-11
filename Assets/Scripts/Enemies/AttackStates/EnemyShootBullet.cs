using CustomUtils;
using FxComponents;
using UnityEngine;

namespace Enemies.EnemiesSharedStates
{
    public class EnemyShootBullet : EnemyAttack
    {
        private readonly Enemy _enemy;
        private int _rounds;
        private float _timer;

        public bool Ended { get; private set; }

        public EnemyShootBullet(Enemy enemy) : base(enemy) => _enemy = enemy;

        public override void Tick()
        {
            base.Tick();
            _timer -= Time.deltaTime;

            if (_timer <= 0f) Shoot();
        }

        public override void FixedTick()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Ended = false;
            _rounds = _enemy.RoundsAmount;
            Shoot();
        }

        private void Shoot()
        {
            _timer = _enemy.ShootRate;
            _rounds--;

            var directions = Utils.GetFanPatternDirections(_enemy.transform, _enemy.BulletsPerRound,
                _enemy.ShootingAngle);

            foreach (var direction in directions)
            {
                var bullet = _enemy.BulletPrefab.Get<Bullet>(_enemy.transform.position + Vector3.up * 0.5f,
                    Quaternion.identity);
                bullet.Setup(direction, _enemy.BulletSpeed, _enemy.Damage);
            }

            SfxManager.Instance.PlayFx(Sfx.BulletShot, _enemy.transform.position);
            if (_rounds <= 0) Ended = true;
        }
    }
}
using CustomUtils;
using UnityEngine;

namespace Enemies.BallShooter
{
    public class BallShooterAttack : EnemyAttack
    {
        private readonly BallShooter _ballShooter;
        private int _rounds;
        private float _timer;

        public bool Ended { get; private set; }

        public BallShooterAttack(BallShooter ballShooter) : base(ballShooter) => _ballShooter = ballShooter;

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
            _rounds = _ballShooter.RoundsAmount;
            Shoot();
        }

        private void Shoot()
        {
            _timer = _ballShooter.ShootRate;
            _rounds--;

            var directions = Utils.GetFanPatternDirections(_ballShooter.transform, _ballShooter.BulletsPerRound,
                _ballShooter.ShootingAngle);

            foreach (var direction in directions)
            {
                var bullet = _ballShooter.BulletPrefab.Get<Bullet>(_ballShooter.transform.position + Vector3.up * 0.5f,
                    Quaternion.identity);
                bullet.Setup(direction, _ballShooter.BulletSpeed, _ballShooter.Damage);
            }

            if (_rounds <= 0) Ended = true;
        }
    }
}
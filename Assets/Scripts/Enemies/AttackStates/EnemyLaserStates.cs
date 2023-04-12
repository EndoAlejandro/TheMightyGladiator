using CustomUtils;
using Enemies.EnemiesSharedStates;
using FxComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies.LaserDudeComponents
{
    public class EnemyLaserTelegraph : EnemyTelegraph
    {
        private readonly Enemy _enemy;
        private readonly LaserVfx[] _lasers;

        private Vector3[] _directions;
        private Vector3 _playerDirection;
        private RaycastHit _hit;

        public EnemyLaserTelegraph(Enemy enemy) : base(enemy)
        {
            _enemy = enemy;
            _directions = new Vector3[_enemy.BulletsPerRound];
            _lasers = new LaserVfx[_enemy.BulletsPerRound];
        }

        public override void Tick()
        {
            base.Tick();

            _directions = Utils.GetFanPatternDirections(_enemy.transform, _enemy.BulletsPerRound,
                _enemy.ShootingAngle);

            _playerDirection =
                Utils.NormalizedFlatDirection(Player.Instance.transform.position, _enemy.transform.position);

            _enemy.transform.forward = Vector3.Lerp(_enemy.transform.forward, _playerDirection,
                Time.deltaTime * _enemy.AttackRotationSpeed);

            for (int i = 0; i < _directions.Length; i++)
            {
                var direction = _directions[i];
                var laser = _lasers[i];
                if (Physics.Raycast(_enemy.transform.position + Vector3.up, direction, out _hit,
                        _enemy.DetectionDistance))
                    laser.SetPosition(1, _hit.point);
                else
                    laser.SetPosition(1,
                        direction * _enemy.DetectionDistance + _enemy.transform.position +
                        Vector3.up);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            for (var i = 0; i < _lasers.Length; i++)
            {
                _lasers[i] = VfxManager.Instance.GetLaserTelegraph().Get<LaserVfx>();
                _lasers[i].SetPosition(0, _enemy.transform.position + Vector3.up);
            }

            SfxManager.Instance.PlayFx(Sfx.Laser, _enemy.transform.position);
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach (var laser in _lasers) laser.TurnOff();
        }
    }

    public class EnemyLaserAttack : StateTimer, IState
    {
        public override string ToString() => "Attack";

        private readonly Enemy _enemy;
        private readonly LaserVfx[] _lasers;

        private Vector3[] _directions;
        private Vector3 _playerDirection;
        private RaycastHit _hit;
        private bool _canDealDamage;

        public EnemyLaserAttack(Enemy enemy)
        {
            _enemy = enemy;
            _directions = new Vector3[_enemy.BulletsPerRound];
            _lasers = new LaserVfx[_enemy.BulletsPerRound];
        }

        public override void Tick()
        {
            base.Tick();

            _directions = Utils.GetFanPatternDirections(_enemy.transform, _enemy.BulletsPerRound,
                _enemy.ShootingAngle);

            _playerDirection =
                Utils.NormalizedFlatDirection(Player.Instance.transform.position, _enemy.transform.position);

            _enemy.transform.forward = Vector3.Lerp(_enemy.transform.forward, _playerDirection,
                Time.deltaTime * _enemy.AttackRotationSpeed);

            for (int i = 0; i < _directions.Length; i++)
            {
                var direction = _directions[i];
                var laser = _lasers[i];
                if (Physics.Raycast(_enemy.transform.position + Vector3.up, direction, out _hit,
                        _enemy.DetectionDistance * 2))
                {
                    laser.SetPosition(1, _hit.point);
                    if (!_hit.transform.TryGetComponent(out Player player) || !_canDealDamage) continue;
                    player.TryToGetDamageFromEnemy(_enemy);
                    _canDealDamage = false;
                }
                else
                {
                    laser.SetPosition(1,
                        direction * _enemy.DetectionDistance * 2 + _enemy.transform.position +
                        Vector3.up);
                }
            }
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _canDealDamage = true;
            timer = .25f;
            for (var i = 0; i < _lasers.Length; i++)
            {
                _lasers[i] = VfxManager.Instance.GetLaserAttack().Get<LaserVfx>();
                _lasers[i].SetPosition(0, _enemy.transform.position + Vector3.up);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach (var laser in _lasers) laser.TurnOff();
        }
    }
}
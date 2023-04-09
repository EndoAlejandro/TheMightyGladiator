using CustomUtils;
using Enemies.EnemiesSharedStates;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.LaserDudeComponents
{
    public class EnemyLaserTelegraph : EnemyTelegraph
    {
        private readonly LaserDude _laserDude;
        private readonly LaserController _laserController;

        private Vector3 _direction;
        private RaycastHit _hit;

        public EnemyLaserTelegraph(LaserDude laserDude, LaserController laserController) : base(laserDude)
        {
            _laserDude = laserDude;
            _laserController = laserController;
        }

        public override void Tick()
        {
            base.Tick();

            _direction =
                Utils.NormalizedFlatDirection(Player.Instance.transform.position, _laserDude.transform.position);
            _laserDude.transform.forward = Vector3.Lerp(_laserDude.transform.forward, _direction,
                Time.deltaTime * _laserDude.AttackRotationSpeed);

            if (Physics.Raycast(_laserDude.transform.position + Vector3.up, _laserDude.transform.forward, out _hit,
                    _laserDude.DetectionDistance))
                _laserController.TelegraphLineRenderer.SetPosition(1, _hit.point);
            else
                _laserController.TelegraphLineRenderer.SetPosition(1,
                    _laserDude.transform.forward * _laserDude.DetectionDistance + _laserDude.transform.position +
                    Vector3.up);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _laserController.TelegraphLineRenderer.SetPosition(0, _laserDude.transform.position + Vector3.up);
            _laserController.SetTelegraphDisplayState(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _laserController.SetTelegraphDisplayState(false);
        }
    }

    public class EnemyLaserAttack : StateTimer, IState
    {
        private readonly LaserDude _laserDude;
        private readonly LaserController _laserController;
        private Vector3 _direction;
        private RaycastHit _hit;
        private float _hitTimer;

        public EnemyLaserAttack(LaserDude laserDude, LaserController laserController)
        {
            _laserDude = laserDude;
            _laserController = laserController;
        }

        public override void Tick()
        {
            _hitTimer -= Time.deltaTime;
            base.Tick();

            _direction =
                Utils.NormalizedFlatDirection(Player.Instance.transform.position, _laserDude.transform.position);
            _laserDude.transform.forward = Vector3.Lerp(_laserDude.transform.forward, _direction,
                Time.deltaTime * _laserDude.AttackRotationSpeed);

            if (Physics.Raycast(_laserDude.transform.position + Vector3.up, _laserDude.transform.forward, out _hit,
                    _laserDude.DetectionDistance))
            {
                if (_hit.transform.TryGetComponent(out Player player) && _hitTimer <= 0f)
                {
                    _hitTimer = 0.5f;
                    player.TryToGetDamageFromEnemy(_laserDude);
                }

                _laserController.AttackLineRenderer.SetPosition(1, _hit.point);
            }
            else
                _laserController.AttackLineRenderer.SetPosition(1,
                    _laserDude.transform.forward * _laserDude.DetectionDistance + _laserDude.transform.position +
                    Vector3.up);
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            timer = 10f;
            _laserController.AttackLineRenderer.SetPosition(0, _laserDude.transform.position + Vector3.up);
            _laserController.SetAttackDisplayState(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _laserController.SetAttackDisplayState(false);
        }
    }
}
using CustomUtils;
using Enemies.BatComponents;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class EnemyPrepareAttack : IState
    {
        private readonly Bat _bat;
        private readonly Player _player;
        private float _timer;
        public bool Ended => _timer <= 0f;

        public EnemyPrepareAttack(Bat bat, Player player)
        {
            _bat = bat;
            _player = player;
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;
            /*var direction = Utils.NormalizedFlatDirection(_player.transform.position, _bat.transform.position);
            _bat.transform.forward = Vector3.Lerp(_bat.transform.forward, direction, Time.deltaTime * _bat.RotationSpeed);*/
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = _bat.PrepareToAttackTime;

        public void OnExit()
        {
        }
    }
}
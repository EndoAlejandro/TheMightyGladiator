using StateMachineComponents;
using UnityEngine;

namespace Enemies.BatComponents
{
    public class BatDeath : IState
    {
        private readonly Bat _bat;
        private float _timer;
        public bool Ended => _timer <= 0f;
        public BatDeath(Bat bat) => _bat = bat;

        public void Tick()
        {
            _timer -= Time.deltaTime;
            if (Ended) GameObject.Destroy(_bat.gameObject);
        }

        public void FixedTick()
        {
        }

        public void OnEnter() => _timer = _bat.DeathTime;
        public void OnExit() => GameObject.Destroy(_bat.gameObject);
    }
}
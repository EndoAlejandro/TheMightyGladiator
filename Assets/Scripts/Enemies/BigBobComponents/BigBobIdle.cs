using StateMachineComponents;
using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBobIdle : IState
    {
        private readonly BigBob _bigBob;
        private float _timer;
        public bool Ended => _timer <= 0f;
        public int NextState { get; private set; }

        public BigBobIdle(BigBob bigBob)
        {
            _bigBob = bigBob;
        }

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _timer = 5f;
            NextState = Random.Range(0, 2);
        }

        public void OnExit()
        {
        }
    }
}
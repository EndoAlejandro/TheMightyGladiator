using CustomUtils;
using PlayerComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.BigBobComponents
{
    public class BigBobStateMachine : FiniteStateBehaviour
    {
        private BigBob _bigBob;
        private Player _player;
        private Rigidbody _rigidbody;

        protected override void StateMachine()
        {
            var idle = new BigBobIdle(_bigBob);
            var attack = new BigBobAttack(_bigBob);

            var initialJump = new BigBobInitialJump(_bigBob, _rigidbody);
            var airJump = new BigBobAirJump(_bigBob, _player);
            var endJump = new BigBobEndJump(_bigBob, _rigidbody);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, initialJump, () => idle.Ended && idle.NextState == 0);
            stateMachine.AddTransition(initialJump, airJump, () => initialJump.Ended);
            stateMachine.AddTransition(airJump, endJump, () => airJump.Ended);
            stateMachine.AddTransition(endJump, idle, () => endJump.Ended);

            stateMachine.AddTransition(idle, attack, () => idle.Ended && idle.NextState == 1);
            stateMachine.AddTransition(attack, idle, () => attack.Ended);
        }

        protected override void References()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bigBob = GetComponent<BigBob>();
            _player = FindObjectOfType<Player>();
        }
    }

    public class BigBobAttack : IState
    {
        private readonly BigBob _bigBob;

        public bool Ended { get; private set; }

        public BigBobAttack(BigBob bigBob)
        {
            _bigBob = bigBob;
        }

        public void Tick()
        {
            Ended = true;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            Ended = false;
            for (int i = 0; i < _bigBob.RoundsAmount; i++)
            {
                var bullet =
                    _bigBob.MortarPrefab.Get<MortarBomb>(_bigBob.transform.position.With(y: 1f), Quaternion.identity);
                var target = _bigBob.transform.position +
                             Random.insideUnitSphere.With(y: 0f).normalized * Random.Range(5f, 10f);
                bullet.Setup(target, 70f);
            }
        }

        public void OnExit()
        {
        }
    }
}
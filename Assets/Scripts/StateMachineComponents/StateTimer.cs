using UnityEngine;

namespace StateMachineComponents
{
    public abstract class StateTimer
    {
        public bool Ended => timer <= 0f;
        protected float timer;
        public virtual void Tick() => timer -= Time.deltaTime;
        public virtual void OnExit() => timer = 0f;
    }
}
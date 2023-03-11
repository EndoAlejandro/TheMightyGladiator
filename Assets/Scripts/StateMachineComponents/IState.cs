namespace StateMachineComponents
{
    public interface IState {
        void Tick();
        void FixedTick();
        void OnEnter();
        void OnExit();
    }
}